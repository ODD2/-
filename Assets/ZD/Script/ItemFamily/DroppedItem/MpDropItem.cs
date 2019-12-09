public class MpDropItem : SingleDropItem
{ 
    protected new void Start()
    {
        base.Start();
        AcquireItem = new MpRecover();
    }
}
