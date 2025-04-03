using Fuse8.BackendInternship.InternalApi.ApiModels;

namespace Fuse8.BackendInternship.InternalApi.Contracts;

interface ICurrencyAPI
{
	/// <summary>
	/// Получает текущий курс для всех валют
	/// </summary>
	/// <param name="baseCurrency">Базовая валюта, относительно которой необходимо получить курс</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Список курсов валют</returns>
	public Task<CurrencyExchangeRate[]> GetAllCurrentCurrenciesAsync(string baseCurrency, CancellationToken cancellationToken);

	/// <summary>
	/// Получает курс для всех валют, актуальный на <paramref name="date"/>
	/// </summary>
	/// <param name="baseCurrency">Базовая валюта, относительно которой необходимо получить курс</param>
	/// <param name="date">Дата, на которую нужно получить курс валют</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Список курсов валют на дату</returns>
	public Task<CurrencyExchangeRateOnDate> GetAllCurrenciesOnDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken);
}