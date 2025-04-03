using Fuse8.BackendInternship.InternalApi.ApiModels;

namespace Fuse8.BackendInternship.InternalApi.Contracts;

public interface ICachedCurrencyAPI
{
	/// <summary>
	/// Получает текущий курс
	/// </summary>
	/// <param name="currencyType">Валюта, для которой необходимо получить курс</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Текущий курс</returns>
	public Task<CurrencyExchangeRate> GetCurrentCurrencyAsync(string currencyType, CancellationToken cancellationToken);

	/// <summary>
	/// Получает курс валюты, актуальный на <paramref name="date"/>
	/// </summary>
	/// <param name="currencyType">Валюта, для которой необходимо получить курс</param>
	/// <param name="date">Дата, на которую нужно получить курс валют</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Курс на дату</returns>
	public Task<CurrencyExchangeRate> GetCurrencyOnDateAsync(string currencyType, DateOnly date, CancellationToken cancellationToken);
}