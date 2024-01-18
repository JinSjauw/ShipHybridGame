using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform HarpoonBase;
    [SerializeField] private Transform HarpoonGun;
    [SerializeField] private float sensitivity;

    [SerializeField] private Transform harpoonPrefab;
    [SerializeField] private Transform harpoonPlaceHolder;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float reloadTime;

    private float lastX;
    private float lastY;

    private float deltaX;
    private float deltaY;
    
    private Vector2 mousePosition;

    private bool canShoot;
    
    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Mouse.current.delta.value;
        
        RotateHarpoon(mousePosition.x * sensitivity, mousePosition.y * -sensitivity);
        
    }
    #endregion

    #region Unity Input Actions

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && canShoot)
        {
            harpoonPlaceHolder.gameObject.SetActive(false);
            canShoot = false;
            //Shoot.
            Rigidbody bulletBody = Instantiate(harpoonPrefab, muzzle.position, Quaternion.identity).GetComponent<Rigidbody>();
            bulletBody.transform.forward = muzzle.forward;
            bulletBody.AddForce(bulletBody.mass * (muzzle.forward * bulletSpeed));

            StartCoroutine(ReloadHarpoon());
        }
    }

    #endregion
    
    #region Private Functions

    private IEnumerator ReloadHarpoon()
    {
        yield return new WaitForSeconds(reloadTime);
        harpoonPlaceHolder.gameObject.SetActive(true);
        canShoot = true;
    }
    
    #endregion

    #region Public Functions

    public void RotateHarpoon(float x, float y)
    {
        HarpoonGun.localRotation *= Quaternion.AngleAxis(y, Vector3.right);
        HarpoonBase.localRotation *= Quaternion.AngleAxis(x, Vector3.up);
    }

    #endregion
}
