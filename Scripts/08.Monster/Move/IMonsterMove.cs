public interface IMonsterMove
{
    // MonsterController와 연결
    void Initialize(OldMonsterController controller);

    // 매 프레임 이동 판단
    void Tick();
}
