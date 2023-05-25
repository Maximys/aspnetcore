// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http.Requests;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http.Tests;

public class RequestCookiesCollectionTests
{
    [Theory]
    [InlineData("key=value", "key", "value")]
    [InlineData("key%2C=%21value", "key%2C", "!value")]
    [InlineData("ke%23y%2C=val%5Eue", "ke%23y%2C", "val^ue")]
    [InlineData("base64=QUI%2BREU%2FRw%3D%3D", "base64", "QUI+REU/Rw==")]
    [InlineData("base64=QUI+REU/Rw==", "base64", "QUI+REU/Rw==")]
    public void UnEscapesValues(string input, string expectedKey, string expectedValue)
    {
        var cookies = RequestCookieCollection.Parse(
            new StringValues(input),
            RequestCookiesSettings.DefaultIsNameCaseInsensitive);

        Assert.Equal(1, cookies.Count);
        Assert.Equal(expectedKey, cookies.Keys.Single());
        Assert.Equal(expectedValue, cookies[expectedKey]);
    }

    public static IEnumerable<object[]> ParseCookies_Data()
    {
        yield return new object[]
        {
            new StringValues(new[]
            {
                "a=Value_a",
                "b=Value_b",
                "c=Value_c",
                "d=Value_d",
                "e=Value_e",
                "f=Value_f",
                "g=Value_g",
                "h=Value_h",
                "i=Value_i",
                "j=Value_j",
                "k=Value_k",
                "l=Value_l"
            }),
            true,
            new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("a", "Value_a"),
                new KeyValuePair<string, string>("b", "Value_b"),
                new KeyValuePair<string, string>("c", "Value_c"),
                new KeyValuePair<string, string>("d", "Value_d"),
                new KeyValuePair<string, string>("e", "Value_e"),
                new KeyValuePair<string, string>("f", "Value_f"),
                new KeyValuePair<string, string>("g", "Value_g"),
                new KeyValuePair<string, string>("h", "Value_h"),
                new KeyValuePair<string, string>("i", "Value_i"),
                new KeyValuePair<string, string>("j", "Value_j"),
                new KeyValuePair<string, string>("k", "Value_k"),
                new KeyValuePair<string, string>("l", "Value_l"),
            }
        };
        yield return new object[]
        {
            new StringValues(new[]
            {
                "a=Value_a",
                "A=Value_A",
            }),
            false,
            new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("a", "Value_a"),
                new KeyValuePair<string, string>("A", "Value_A"),
            }
        };
    }

    [Theory]
    [MemberData(nameof(ParseCookies_Data))]
    public void ParseCookies(
        StringValues values,
        bool isNameCaseInsensitive,
        List<KeyValuePair<string, string>> expectedValues)
    {
        var cookies = RequestCookieCollection.Parse(values, isNameCaseInsensitive);

        Assert.Equivalent(expectedValues, cookies, true);
    }

    [Theory]
    [InlineData(",", null)]
    [InlineData(";", null)]
    [InlineData("er=dd,cc,bb", new[] { "dd" })]
    [InlineData("er=dd,err=cc,errr=bb", new[] { "dd", "cc", "bb" })]
    [InlineData("errorcookie=dd,:(\"sa;", new[] { "dd" })]
    [InlineData("s;", null)]
    public void ParseInvalidCookies(string cookieToParse, string[] expectedCookieValues)
    {
        var cookies = RequestCookieCollection.Parse(
            new StringValues(new[] { cookieToParse }),
            RequestCookiesSettings.DefaultIsNameCaseInsensitive);

        if(expectedCookieValues == null)
        {
            Assert.Equal(0, cookies.Count);
            return;
        }

        Assert.Equal(expectedCookieValues.Length, cookies.Count);
        for (int i = 0; i < expectedCookieValues.Length; i++)
        {
            var value = expectedCookieValues[i];
            Assert.Equal(value, cookies.ElementAt(i).Value);
        }
    }
}
