﻿// Copyright The ORAS Authors.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net.Http;

namespace OrasProject.Oras.Registry.Remote;

/// <summary>
/// DefaultHttpClient is to provide a single lazy-loading HttpClient across the application context to reduce the creation of http client pools
/// </summary>
internal class DefaultHttpClient
{
    private static readonly Lazy<HttpClient> _client =
        new(() => new HttpClient());

    internal static HttpClient Instance => _client.Value;
}
