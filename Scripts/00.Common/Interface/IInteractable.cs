
// 상호작용하는 모든 오브젝트에 붙일 인터페이스
// 상호작용 주체는 Player이기 때문에 Player 클래스 정보를 전달한다.
// 상호작용은 우선순위를 가지고 있어 상호작용 오브젝트가 겹쳤을 때는 우선순위로 해결한다.

using UnityEngine;

public interface IInteractable
{
    public void Interact(Player player);
    public int GetPriority();
    public Vector3 GetPosition();
    public void OpenInteractUI(Player player);
    public void CloseInteractUI(Player player);
}
