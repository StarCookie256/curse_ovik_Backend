using PerfumeryBackend.ParserLayer.Interfaces;

namespace PerfumeryBackend.MainLayer.Services;

public class StartupParserService(
    IServiceProvider _serviceProvider,
    ILogger<StartupParserService> _logger,
    IHostApplicationLifetime _appLifetime) : IHostedService
{

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartupParserService запущен");

        try
        {
            // Создаем scope для получения scoped сервисов
            using var scope = _serviceProvider.CreateScope();
            var parser = scope.ServiceProvider.GetRequiredService<IParserService>();

            await parser.ParseData();

            _logger.LogInformation("Парсинг при запуске выполнен успешно");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Критическая ошибка при запуске парсера");

            // Останавливаем приложение при критической ошибке
            _appLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 StartupParserService остановлен");
        return Task.CompletedTask;
    }
}
