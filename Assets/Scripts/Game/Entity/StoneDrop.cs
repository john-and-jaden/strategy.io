public class StoneDrop : ResourceDrop
{
    protected override void Collect()
    {
        GameManager.ResourceSystem.AddStone(resourceAmount);
        base.Collect();
    }
}
