using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public Transform targetObject; // Объект, на который нужно смотреть
    public float requiredAngle = 5f; // Допустимый угол отклонения
    public float requiredTime = 5f; // Время, которое нужно смотреть
    public GameObject reward; // Награда 

    private bool isPlayerInZone = false;
    private float timer = 0f;

    void Update()
    {
        if (isPlayerInZone)
        {
            // Получаем направление камеры и направление на объект
            Vector3 cameraDirection = Camera.main.transform.forward;
            Vector3 targetDirection = (targetObject.position - Camera.main.transform.position).normalized;

            // Вычисляем угол между направлением камеры и направлением на объект
            float angle = Vector3.Angle(cameraDirection, targetDirection);

            // Проверяем, находится ли угол в допустимых пределах
            if (angle <= requiredAngle)
            {
                timer += Time.deltaTime; // Увеличиваем таймер
                print(timer);
                if (timer >= requiredTime)
                {
                    Debug.Log("Правильный угол и время! Награда активирована.");
                    ActivateReward();
                }
            }
            else
            {
                timer = 0f; // Сбрасываем таймер, если угол неправильный
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            timer = 0f; // Сбрасываем таймер, если игрок покидает зону
        }
    }

    private void ActivateReward()
    {
        if (reward != null)
        {
            reward.SetActive(true); // Активируем награду
        }
        enabled = false; // Отключаем скрипт после активации награды
    }
}
