﻿using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using YouTrack.Rest.Requests.Projects;

namespace YouTrack.Rest.Tests
{
    class ProjectTests : TestFor<Project>
    {
        private IConnection connection;
        private Rest.Deserialization.Project deserializedProject;
        private const string ProjectId = "FOO";

        protected override Project CreateSut()
        {
            connection = Mock<IConnection>();
            return new Project(ProjectId, connection);
        }

        protected override void SetupDependencies()
        {
            deserializedProject = Mock<Rest.Deserialization.Project>();
            connection.Get<Rest.Deserialization.Project>(Arg.Any<GetProjectRequest>()).Returns(Task.Factory.StartNew(() => deserializedProject));
        }

        [Test]
        public void ProjectIsLoaded()
        {
            Sut.Load().Wait();

            connection.Received().Get<Rest.Deserialization.Project>(Arg.Any<GetProjectRequest>());
        }

        [Test]
        public void ProjectIsMapped()
        {
            Sut.Load().Wait();

            deserializedProject.Received().MapTo(Sut);
        }
    }
}