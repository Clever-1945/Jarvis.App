## Jarvis.App

![Image](./screenshots/MainWindow.png)

Приложение основано на плагинах
При помощи текстового ввода мы отправляем запрос каждому плагину. 
Плагин его обрабатывает и отображается результат.

Показан пример работы "Jarvis.Google.Plugin"


Это приложение считается альтернативой Wox, Alfred


Для создания плагина надо
1. Создать проект .NET
2. Добавить зависимость в проект Jarvis.Plugins
3. Создать класс, унаследоваться от JItemPlugin, пометить класс аттрибутом JPluginInfo

Пример:
```csharp
[JPluginInfo("UID", "{имя плагина}", "{Описание плагина}")]
public class Plugin: JItemPlugin
{    
    public void Init(IHostService hostService)
    {
        _hostService = hostService;
    }

    public void Request(RequestPlugin request, ResponseProcessor processor)
    {
        processor.ShowItem?.Invoke(new ResponsePlugin()
        {
            Item = new ItemPlugin()
            {
                Id = Guid.NewGuid(),
                Data = null,    // Данные, которые вам могут потребоваться при выборе элемента
                Request = request,
                IconData = null,    // Бинарные данные картинки
                Text = text,
                Description = description,
                Trigger = (i) =>
                {
                    // что делать когда пользователь выберит элемент плагина
                }
            }
        });
    }
}
```

4. Рядом с приложением должна быт папка Plugins
5. В папке Plugins должна быть папка с названием плагина, в которой должны быть все зависимости вашего плагина
