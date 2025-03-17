namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Контейнер для значения, с отложенным получением
/// </summary>
public class Lazy<TValue>
{
	private readonly Func<TValue>? _act;
    private bool _isLazy = false;
    private TValue? _value;
    // ToDo: Реализовать ленивое получение значение при первом обращении к Value

    public Lazy(Func<TValue>? act)
    {
        if(act == null)
            throw new ArgumentNullException(nameof(act));
        _act = act;
    }

    public TValue? Value
    {
        get
        {
            if (!_isLazy)
            {
                _value = _act();
                _isLazy = true; 
            }
            return _value; 
        }
    }
}