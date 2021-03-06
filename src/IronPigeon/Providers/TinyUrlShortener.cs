﻿namespace IronPigeon.Providers {
	using System;
	using System.Collections.Generic;
	using System.Composition;
	using System.Globalization;
	using System.Linq;
	using System.Net.Http;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Validation;

	/// <summary>
	/// A URL shortener that uses tinyurl.com
	/// </summary>
	[Export(typeof(IUrlShortener))]
	[Export]
	[ExportMetadata("SupportsUnusualSchemes", true)]
	[Shared]
	public class TinyUrlShortener : IUrlShortener {
		/// <summary>
		/// The template for the URL of the service that takes a long URL and returns a short one.
		/// </summary>
		protected const string ShorteningService = "http://tinyurl.com/api-create.php?url={0}";

		/// <summary>
		/// Gets or sets the HTTP client to use for outbound HTTP requests.
		/// </summary>
		[Import]
		public HttpClient HttpClient { get; set; }

		/// <summary>
		/// Shortens the asynchronous.
		/// </summary>
		/// <param name="longUrl">The long URL.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task whose result is the shortened URL.</returns>
		public async Task<Uri> ShortenAsync(Uri longUrl, CancellationToken cancellationToken) {
			Requires.NotNull(longUrl, "longUrl");

			string shorteningRequestUrl = string.Format(
				CultureInfo.InvariantCulture, ShorteningService, Uri.EscapeDataString(longUrl.AbsoluteUri));
			var response = await this.HttpClient.GetAsync(shorteningRequestUrl, cancellationToken);
			response.EnsureSuccessStatusCode();
			string responseAsString = await response.Content.ReadAsStringAsync();
			return new Uri(responseAsString, UriKind.Absolute);
		}
	}
}
