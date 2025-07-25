// Copyright The ORAS Authors.
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
using Moq;
using OrasProject.Oras;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasProject.Oras.Registry.Remote;
using OrasProject.Oras.Registry;

public class CopyArtifact
{
    public async Task CopyArtifactAsync()
    {
        #region Usage
        // This example demonstrates how to copy an artifact from one repository to another.

        // Create a HttpClient instance to be used for making HTTP requests.
        var httpClient = new HttpClient();

        // Source repository
        var sourceCred = new Mock<ICredentialProvider>();
        var sourceRepository = new Repository(new RepositoryOptions
        {
            Reference = Reference.Parse("source.io/testrepository"),
            Client = new Client(httpClient, credentialProvider: sourceCred.Object),
        });

        // Destination repository
        var destinationCred = new Mock<ICredentialProvider>();
        var destRepository = new Repository(new RepositoryOptions
        {
            Reference = Reference.Parse("target.io/testrepository"),
            Client = new Client(httpClient, credentialProvider: destinationCred.Object)
        });

        // Copy the artifact tagged by reference from the source repository to the destination
        var reference = "tag";
        var rootDescriptor = await sourceRepository.CopyAsync(reference, destRepository, "");
        #endregion
    }
}
