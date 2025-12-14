namespace Jarvis.Plugins;

public interface JItemPlugin
{
    /// <summary>
    /// Инициализация плагина
    /// Вызывается когда библиотека с плагином загружается в память и создается экзэмпляр класса плагина
    /// </summary>
    void Init(IHostService hostService);

    /// <summary> Вызывается каждый раз когда меняется запрос в строке поиска </summary>
    /// <param name="request"></param>
    /// <param name="processor"></param>
    void Request(RequestPlugin request, ResponseProcessor processor);
}