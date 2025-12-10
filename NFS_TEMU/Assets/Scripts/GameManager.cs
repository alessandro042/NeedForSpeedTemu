using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<MenuCharacters> vehiculos;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (scene.name == "SampleScene")
        {
            SpawnSelectedVehicle();
        }
    }

    private void SpawnSelectedVehicle()
    {
        int index = PlayerPrefs.GetInt("JugadorIndex", 0);
        if (vehiculos == null || vehiculos.Count == 0)
        {
            Debug.LogWarning("GameManager: No hay vehículos configurados en 'vehiculos'. Asegura llenar la lista en el Inspector.");
            return;
        }

        if (index < 0 || index >= vehiculos.Count)
        {
            Debug.LogWarning($"GameManager: JugadorIndex ({index}) fuera de rango, reseteando a 0.");
            index = 0;
        }

        var prefab = vehiculos[index].vehiculoJugable;
        if (prefab == null)
        {
            Debug.LogWarning($"GameManager: El vehiculoJugable del MenuCharacters en index {index} es null.");
            return;
        }

        
        var spawnPoint = GameObject.Find("PlayerSpawn");
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        GameObject existing = null;
        
        if (spawnPoint != null)
        {
            pos = spawnPoint.transform.position;
            rot = spawnPoint.transform.rotation;
        }
        else
        {
            existing = GameObject.FindGameObjectWithTag("Player");
            if (existing != null)
            {
                pos = existing.transform.position;
                rot = existing.transform.rotation;
            }
            else
            {
                pos = new Vector3(0, 1, 0);
                rot = Quaternion.identity;
            }
        }

        
        var instance = Instantiate(prefab, pos, rot);
        try
        {
            instance.tag = "Player";
        }
        catch { }

        
        var rbInst = instance.GetComponent<Rigidbody>();
        if (rbInst == null)
        {
            rbInst = instance.AddComponent<Rigidbody>();
            rbInst.mass = 1f;
            rbInst.interpolation = RigidbodyInterpolation.Interpolate;
            rbInst.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rbInst.isKinematic = false;
            rbInst.linearDamping = 0.1f;
            rbInst.angularDamping = 0.05f;
            rbInst.constraints = RigidbodyConstraints.None;
            Debug.Log($"GameManager: Se añadió Rigidbody al vehículo instanciado '{instance.name}' (fallback).");
        }
        else
        {
            
            if (rbInst.isKinematic)
            {
                rbInst.isKinematic = false;
                Debug.Log($"GameManager: Rigidbody en '{instance.name}' tenía isKinematic=true; se cambió a false.");
            }
            
            if (rbInst.linearDamping > 1f) rbInst.linearDamping = 0.1f;
            if (rbInst.angularDamping > 1f) rbInst.angularDamping = 0.05f;
            if (rbInst.constraints != RigidbodyConstraints.None)
            {
                rbInst.constraints = RigidbodyConstraints.None;
                Debug.Log($"GameManager: Rigidbody constraints en '{instance.name}' fueron removidos para permitir movimiento.");
            }
        }

        
        var cc = instance.GetComponent<CarController>();
        if (cc == null)
        {
            cc = instance.AddComponent<CarController>();
            Debug.Log($"GameManager: Se añadió CarController al vehículo instanciado '{instance.name}' (fallback). Revisa que los wheels/frontWheels estén configurados en el prefab para mejores resultados.");
        }

        
        var anyCollider = instance.GetComponentsInChildren<Collider>(true).Length > 0;
        if (!anyCollider)
        {
            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers != null && renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
                var box = instance.AddComponent<BoxCollider>();
                box.center = instance.transform.InverseTransformPoint(bounds.center);
                
                var sizeLocal = instance.transform.InverseTransformVector(bounds.size);
                box.size = new Vector3(Mathf.Abs(sizeLocal.x), Mathf.Abs(sizeLocal.y), Mathf.Abs(sizeLocal.z));
                Debug.Log($"GameManager: Se añadió BoxCollider al vehículo '{instance.name}' con size {box.size} (fallback).");
            }
            else
            {
                
                var box = instance.AddComponent<BoxCollider>();
                box.size = Vector3.one * 2f;
                Debug.Log($"GameManager: No se encontraron renderers en '{instance.name}'; se añadió BoxCollider genérico.");
            }
        }

        
        var camSystem = FindObjectOfType<CameraSystem>();
        if (camSystem != null)
        {
            camSystem.target = instance.transform;
            Debug.Log($"GameManager: CameraSystem.target reasignado a {instance.name}");
        }

        
        if (existing != null)
        {
            
            var monos = existing.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var m in monos)
            {
                try { m.enabled = false; } catch { }
            }
            existing.SetActive(false);
            StartCoroutine(DestroyAfterFrame(existing));
        }

        Debug.Log($"GameManager: Vehículo instanciado: {instance.name} (index {index}) en escena '{SceneManager.GetActiveScene().name}'.");

        
        StartCoroutine(CheckMovement(instance, rbInst));
    }

    private IEnumerator DestroyAfterFrame(GameObject obj)
    {
        
        yield return null;
        if (obj != null)
        {
            Destroy(obj);
            Debug.Log($"GameManager: Objeto antiguo '{obj.name}' destruido después de un frame.");
        }
    }

    private IEnumerator CheckMovement(GameObject instance, Rigidbody rbInst)
    {
        
        yield return new WaitForSeconds(0.5f);
        if (rbInst == null) yield break;
        if (rbInst.linearVelocity.magnitude < 0.1f)
        {
            
            rbInst.AddForce(instance.transform.forward * 2f, ForceMode.VelocityChange);
            Debug.Log($"GameManager: Debug -> Se aplicó impulso de prueba al vehículo '{instance.name}' porque su velocidad era {rbInst.linearVelocity.magnitude}.");

            
            yield return new WaitForSeconds(0.25f);
            if (rbInst.linearVelocity.magnitude < 0.1f)
            {
                
                rbInst.WakeUp();

                Debug.LogError($"GameManager: DEBUG EXTENDIDO -> El vehículo '{instance.name}' sigue sin moverse tras el impulso. Recolectando info de diagnóstico...");
                Debug.Log($"GameManager: Posición='{instance.transform.position}', escala='{instance.transform.localScale}', layer={LayerMask.LayerToName(instance.layer)} ({instance.layer})");
                Debug.Log($"GameManager: Rigidbody: isKinematic={rbInst.isKinematic}, sleeping={rbInst.IsSleeping()}, velocity={rbInst.linearVelocity}, mass={rbInst.mass}, drag={rbInst.linearDamping}, angularDrag={rbInst.angularDamping}, constraints={rbInst.constraints}");

                
                var cols = instance.GetComponentsInChildren<Collider>(true);
                Debug.Log($"GameManager: Colliders encontrados en el vehículo: {cols.Length}");
                foreach (var c in cols)
                {
                    if (c == null) continue;
                    Debug.Log($"  Collider: '{c.gameObject.name}' enabled={c.enabled} isTrigger={c.isTrigger} attachedRigidbody={(c.attachedRigidbody!=null ? c.attachedRigidbody.gameObject.name : "null")}");
                }

                
                var cur = instance.transform.parent;
                while (cur != null)
                {
                    var prb = cur.GetComponent<Rigidbody>();
                    if (prb != null)
                    {
                        Debug.Log($"GameManager: Rigidbody en padre '{cur.name}' encontrado: isKinematic={prb.isKinematic}, constraints={prb.constraints}");
                    }
                    cur = cur.parent;
                }

                
                var anims = instance.GetComponentsInChildren<Animator>(true);
                Debug.Log($"GameManager: Animators en el vehículo: {anims.Length}");

                var monos = instance.GetComponents<MonoBehaviour>();
                Debug.Log($"GameManager: MonoBehaviours en root del vehículo ({monos.Length}):");
                foreach (var m in monos)
                {
                    if (m == null) continue;
                    Debug.Log($"  {m.GetType().Name} enabled={m.enabled}");
                }

                Debug.LogError("GameManager: FIN DEBUG EXTENDIDO. Revisa las entradas anteriores para identificar por qué el vehículo no se mueve (colliders triggers, Rigidbody kinematic, constraints, Animator/Script que fija transform, o escala inusual). Sube la salida si quieres que lo analice.");
            }
            else
            {
                Debug.Log($"GameManager: Debug -> Tras impulso, velocidad del vehículo '{instance.name}' es {rbInst.linearVelocity.magnitude}.");
            }
        }
    }
}
