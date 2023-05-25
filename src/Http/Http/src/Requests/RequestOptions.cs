// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Http.Requests;

/// <summary>
/// Options to configure request processing.
/// </summary>
public class RequestOptions
{
    /// <summary>
    /// Settings for cookies.
    /// </summary>
    public RequestCookiesSettings CookiesSettings { get; } = RequestCookiesSettings.Default;
}
