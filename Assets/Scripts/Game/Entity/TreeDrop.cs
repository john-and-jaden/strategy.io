public class TreeDrop : ResourceDrop
{
    protected override void Collect()
    {
        GameManager.ResourceSystem.AddWood(resourceAmount);
        base.Collect();
    }
}
