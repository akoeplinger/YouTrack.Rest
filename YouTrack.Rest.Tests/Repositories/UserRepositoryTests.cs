using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Repositories;
using YouTrack.Rest.Requests.Users;

namespace YouTrack.Rest.Tests.Repositories
{
    class UserRepositoryTests : TestFor<UserRepository>
    {
        private IConnection connection;

        protected override UserRepository CreateSut()
        {
            connection = Mock<IConnection>();

            return new UserRepository(connection);
        }

        [Test]
        public void CreateANewUserRequestIsUsed()
        {
            connection.Put(Arg.Any<CreateANewUserRequest>()).Returns(Task.Factory.StartNew(() => ""));

            Sut.CreateUser("login", "password", "email", "fullName");

            connection.Received().Put(Arg.Any<CreateANewUserRequest>());
        }

        [Test]
        public void DeleteUserRequestIsUsed()
        {
            connection.Delete(Arg.Any<DeleteUserRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.DeleteUser("login");

            connection.Received().Delete(Arg.Any<DeleteUserRequest>());
        }


        [Test]
        public void GetUserRequestIsUsed()
        {
            connection.Get<Rest.Deserialization.User>(Arg.Any<GetUserRequest>()).Returns(Task.Factory.StartNew(() => new Rest.Deserialization.User()));

            Sut.GetUser("foobar");

            connection.Received().Get<Rest.Deserialization.User>(Arg.Any<GetUserRequest>());
        }

        [Test]
        public void UserExists()
        {
            connection.Get(Arg.Any<GetUserRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.UserExists("foobar");

            connection.Received().Get(Arg.Any<GetUserRequest>());
        }

        [Test]
        public void UserDoesNotExist()
        {
            connection.Get(Arg.Any<GetUserRequest>()).Returns(Task.Factory.StartNew(() =>
                                                                                        {
                                                                                            throw new RequestNotFoundException(Mock<IRestResponse>());
                                                                                        }));

            Assert.IsFalse(Sut.UserExists("foo").Result);
        }
    }
}
