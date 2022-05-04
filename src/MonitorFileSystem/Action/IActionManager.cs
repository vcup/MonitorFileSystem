namespace MonitorFileSystem.Action;

public interface IActionManager : IDictionary<Guid, IOperate>, ICollection<IChain>
{
    Guid Add(IOperate operate);

    bool Contains(string name);
    IChain? Find(string name);
    
    IEnumerable<IChain> Chains { get; }
    IEnumerable<IOperate> Operates { get; }

    void ClearUp();
}
