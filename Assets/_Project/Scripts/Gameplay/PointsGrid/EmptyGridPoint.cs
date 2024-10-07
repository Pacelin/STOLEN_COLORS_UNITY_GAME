namespace Audio.Gameplay.PointsGrid
{
    public class EmptyGridPoint : GridPoint
    {
        protected override void Awake() =>
            gameObject.SetActive(false);
    }
}