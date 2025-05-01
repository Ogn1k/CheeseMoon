using KinematicCharacterController;
using UnityEngine;

public class WaterBounce : MonoBehaviour
{
    [Tooltip("Слои")]
    public LayerMask WaterLayer;
    public LayerMask GroundLayer;
    [Tooltip("Все остальное")]
    public float AutoJumpImpulse = 10f; // Сила импульса прыжка
    //public float JumpCooldown = 0.1f; // Время между прыжками
    public float TeleportSearchRadius = 20f;

    private KinematicCharacterMotor _motor;
    private float _jumpTimer;
    private int _jumpCount;

    private void Awake()
    {
        _motor = GetComponent<KinematicCharacterMotor>();
    }

    private void Update()
    {
        //print(_motor.GroundingStatus.GroundCollider.gameObject.layer);
        // Если персонаж приземлён и имеет коллайдер (GroundCollider)
        if (_motor.GroundingStatus.IsStableOnGround && _motor.GroundingStatus.GroundCollider != null)
        {
            // Проверяем, что объект, на котором стоит игрок, относится к слою Water
            if (((1 << _motor.GroundingStatus.GroundCollider.gameObject.layer) & WaterLayer) != 0)
            {
                AutoJump();
            }
            else
            {
                // Если персонаж стоит не на воде, сбрасываем счетчик прыжков
                _jumpCount = 0;
            }
        }
    }

	private void AutoJump()
    {
        // Принудительное отрывание от земли
        _motor.ForceUnground();

        // Задаем импульс вверх (используем текущий CharacterUp)
        Vector3 jumpVelocity = _motor.CharacterUp * AutoJumpImpulse;
        _motor.BaseVelocity += jumpVelocity;
        
        _jumpCount++;
        Debug.Log("Прыжков с воды: " + _jumpCount);
        
        if (_jumpCount >= 3)
        {
            TeleportToNearestGround();
            _jumpCount = 0;
        }
    }

    private void TeleportToNearestGround()
    {
        // Поиск коллайдеров из слоя Ground в пределах указанного радиуса
        Collider[] colliders = Physics.OverlapSphere(transform.position, TeleportSearchRadius, GroundLayer);
        if (colliders.Length == 0)
        {
            Debug.LogWarning("Не найдено поверхности для телепортации.");
            return;
        }

        Vector3 currentPosition = transform.position;
        float closestDistance = Mathf.Infinity;
        Vector3 closestPoint = currentPosition;

        // Ищем ближайшую точку среди найденных коллайдеров
        foreach (Collider col in colliders)
        {
            Vector3 point = col.ClosestPoint(currentPosition);
            float dist = Vector3.Distance(currentPosition, point);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestPoint = point;
            }
        }

        // Телепортируем персонажа к найденной точке (с небольшим смещением вверх)
        _motor.SetTransientPosition( closestPoint + _motor.CharacterUp * 0.5f);
        Debug.Log("Телепортация на позицию: " + transform.position);
    }
}
