public class HpDropItem : SingleDropItem
{
    protected new void Start()
    {
        base.Start();
        AcquireItem = new HpRecover();
    }
}
