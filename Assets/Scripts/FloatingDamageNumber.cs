using System;
using TMPro;
using UnityEngine;

namespace Game.Battle
{
    public class FloatingDamageNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text damageAmount;
        [SerializeField] private Renderer rndr;
        [SerializeField] private bool moveUp = true;
        [SerializeField] private float moveSpeed;

        public void InitializeNumber(int damageAmount, Color color, float killAfterTime)
        {
            this.damageAmount.text = damageAmount.ToString();
            this.damageAmount.color = color;
            Material newMat = rndr.material;
            newMat.color = color;
            rndr.material = newMat;
            Invoke(nameof(KillObject), killAfterTime);
        }

        public void InitializeText(string text, Color color, float killAfterTime)
        {
            this.damageAmount.text = text;
            this.damageAmount.color = color;
            Material newMat = rndr.material;
            newMat.color = color;
            rndr.material = newMat;
            Invoke(nameof(KillObject), killAfterTime);
        }

        private void Update()
        {
            if (moveUp)
            {
                transform.position += (Vector3.up * moveSpeed * Time.deltaTime);
            }
        }

        private void KillObject()
        {
            Destroy(this.gameObject);
        }
    }
}