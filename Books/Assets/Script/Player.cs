using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class Player : MonoBehaviour
    {
        [Header("Status")] public float speed = 50;
        public float camSpeed = 50;
        public float handRange = 5;

        [SerializeField]
        private Book _holdingBook;

        [SerializeField] private Camera cam;
        [SerializeField] private Transform _holdingBookPos;
        private Rigidbody rigid;
        private RaycastHit hit;

        private float xRotation = 0f; // 상하 각도 누적값


        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            CamMove();
            Move();
            Mouse();
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward * handRange, Color.red);
        }

        private void CamMove()
        {
            float mouseX = Input.GetAxis("Mouse X") * camSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * camSpeed * Time.deltaTime;

            // 상하 회전 (X축)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -70f, 70f);

            // 좌우 회전 (Y축) - 플레이어 본체 회전
            transform.Rotate(Vector3.up * mouseX);

            // 카메라 상하만 적용
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        private void Move()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 dir = transform.right * h + transform.forward * v;
            dir = Vector3.ClampMagnitude(dir, 1f) * speed;
            rigid.linearVelocity = new Vector3(dir.x,rigid.linearVelocity.y,dir.z);

            if (_holdingBook != null)
            {
                _holdingBook.transform.position = _holdingBookPos.position;
                _holdingBook.transform.rotation = _holdingBookPos.rotation;
            }
        }

        private void Mouse()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, handRange,
                    LayerMask.GetMask("Interact")))
            {
                var slot = hit.collider.GetComponent<BookSlot>();
                if (slot == null) return;
                if (Input.GetMouseButtonDown(0) && _holdingBook == null && slot.IsOccupied)
                {
                    _holdingBook = slot.TakeBook();
                    Debug.Log(_holdingBook.name);
                }
                else if (Input.GetMouseButtonDown(1) && _holdingBook != null && slot.AddBook(_holdingBook))
                {
                    _holdingBook = null;
                }
            }
        }
    }
}