using UnityEngine;

public interface IPlayerController
{
    void Initialize(GameCharacter character, float speed, bool isHunter);
    void UpdateController();
    void FixedUpdateController();
    void OnMove(Vector2 input);
    void Deactivate();
}
