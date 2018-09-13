﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using McMaster.Extensions.CommandLineUtils;
using Moq;
using Polyrific.Catapult.Cli.Commands;
using Polyrific.Catapult.Cli.Commands.Member;
using Polyrific.Catapult.Shared.Dto.Project;
using Polyrific.Catapult.Shared.Dto.ProjectMember;
using Polyrific.Catapult.Shared.Dto.User;
using Polyrific.Catapult.Shared.Service;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Polyrific.Catapult.Cli.UnitTests.Commands
{
    public class MemberCommandTests
    {
        private readonly Mock<IConsole> _console;
        private readonly Mock<IProjectService> _projectService;
        private readonly Mock<IProjectMemberService> _projectMemberService;
        private readonly Mock<IAccountService> _accountService;

        public MemberCommandTests()
        {
            var projects = new List<ProjectDto>
            {
                new ProjectDto
                {
                    Id = 1,
                    Name = "Project 1"
                }
            };

            var projectMembers = new List<ProjectMemberDto>
            {
                new ProjectMemberDto
                {
                    Id = 1,
                    UserId = 1,
                    ProjectId = 1,
                    ProjectMemberRoleId = 1
                }
            };

            var users = new List<UserDto>
            {
                new UserDto
                {
                    Id = "1",
                    UserName = "user1@opencatapult.net",
                    Email = "user1@opencatapult.net"
                }
            };

            _console = new Mock<IConsole>();

            _projectService = new Mock<IProjectService>();
            _projectService.Setup(p => p.GetProjectByName(It.IsAny<string>())).ReturnsAsync((string name) => projects.FirstOrDefault(p => p.Name == name));

            _projectMemberService = new Mock<IProjectMemberService>();
            _projectMemberService.Setup(p => p.CreateProjectMember(It.IsAny<int>(), It.IsAny<NewProjectMemberDto>())).ReturnsAsync((int projectId, NewProjectMemberDto dto) =>
            {
                var newProjectMember = new ProjectMemberDto
                {
                    Id = 2,
                    ProjectId = projectId,
                    UserId = dto.UserId,
                    ProjectMemberRoleId = dto.ProjectMemberRoleId
                };
                projectMembers.Add(newProjectMember);
                return newProjectMember;
            });
            _projectMemberService.Setup(p => p.GetProjectMembers(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(projectMembers);
            _projectMemberService.Setup(p => p.GetProjectMemberByUserId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((int projectId, int userId) =>
                projectMembers.FirstOrDefault(p => p.ProjectId == projectId && p.UserId == userId));

            _accountService = new Mock<IAccountService>();
            _accountService.Setup(s => s.GetUserByUserName(It.IsAny<string>())).ReturnsAsync((string userName) => users.FirstOrDefault(u => u.UserName == userName));
        }

        [Fact]
        public void Member_Execute_ReturnsEmpty()
        {
            var command = new MemberCommand(_console.Object, LoggerMock.GetLogger<MemberCommand>().Object);
            var resultMessage = command.Execute();

            Assert.Equal("", resultMessage);
        }

        [Fact]
        public void MemberAdd_Execute_ReturnsSuccessMessage()
        {
            var command = new AddCommand(_console.Object, LoggerMock.GetLogger<AddCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 1",
                User = "user1@opencatapult.net"
            };

            var resultMessage = command.Execute();

            Assert.StartsWith("User user1@opencatapult.net is added to project Project 1 as Member:", resultMessage);
        }

        [Fact]
        public void MemberAdd_Execute_NewUserReturnsSuccessMessage()
        {
            var command = new AddCommand(_console.Object, LoggerMock.GetLogger<AddCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 1",
                User = "user2@opencatapult.net",
                Role = "Owner"
            };

            var resultMessage = command.Execute();

            Assert.StartsWith("User user2@opencatapult.net is added to project Project 1 as Owner:", resultMessage);
        }

        [Fact]
        public void MemberAdd_Execute_ReturnsNotFoundMessage()
        {
            var command = new AddCommand(_console.Object, LoggerMock.GetLogger<AddCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 2",
                User = "user2@opencatapult.net",
                Role = "Owner"
            };

            var resultMessage = command.Execute();

            Assert.Equal("Project Project 2 is not found", resultMessage);
        }

        [Fact]
        public void MemberList_Execute_ReturnsSuccessMessage()
        {
            var command = new ListCommand(_console.Object, LoggerMock.GetLogger<ListCommand>().Object, _projectMemberService.Object, _projectService.Object)
            {
                Project = "Project 1"
            };

            var resultMessage = command.Execute();

            Assert.StartsWith("Project members:", resultMessage);
            _projectMemberService.Verify(p => p.GetProjectMembers(1, 0), Times.Once);
        }

        [Fact]
        public void MemberList_Execute_ReturnsNotFoundMessage()
        {
            var command = new ListCommand(_console.Object, LoggerMock.GetLogger<ListCommand>().Object, _projectMemberService.Object, _projectService.Object)
            {
                Project = "Project 2"
            };

            var resultMessage = command.Execute();

            Assert.Equal("Project Project 2 is not found", resultMessage);
        }

        [Fact]
        public void MemberRemove_Execute_ReturnsSuccessMessage()
        {
            var command = new RemoveCommand(_console.Object, LoggerMock.GetLogger<RemoveCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 1",
                User = "user1@opencatapult.net"
            };

            var resultMessage = command.Execute();

            Assert.Equal("User user1@opencatapult.net was removed from project Project 1", resultMessage);
        }

        [Fact]
        public void MemberRemove_Execute_ReturnsNotFoundMessage()
        {
            var command = new RemoveCommand(_console.Object, LoggerMock.GetLogger<RemoveCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 1",
                User = "user2@opencatapult.net"
            };

            var resultMessage = command.Execute();

            Assert.Equal("Failed removing user user2@opencatapult.net. Make sure the project name and user email are correct.", resultMessage);
        }

        [Fact]
        public void MemberUpdate_Execute_ReturnsSuccessMessage()
        {
            var command = new UpdateCommand(_console.Object, LoggerMock.GetLogger<UpdateCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 1",
                User = "user1@opencatapult.net",
                Role = "Owner"
            };

            var resultMessage = command.Execute();

            Assert.Equal("User user1@opencatapult.net is updated to role Owner in project Project 1", resultMessage);
            _projectMemberService.Verify(p => p.UpdateProjectMember(1, 1, It.Is<UpdateProjectMemberDto>(pm => pm.ProjectMemberRoleId == 1)), Times.Once);
        }

        [Fact]
        public void MemberUpdate_Execute_ReturnsNotFoundMessage()
        {
            var command = new UpdateCommand(_console.Object, LoggerMock.GetLogger<UpdateCommand>().Object, _projectMemberService.Object, _projectService.Object, _accountService.Object)
            {
                Project = "Project 1",
                User = "user2@opencatapult.net",
                Role = "Owner"
            };

            var resultMessage = command.Execute();

            Assert.Equal("Failed updating user user2@opencatapult.net. Make sure the project name and user email are correct.", resultMessage);
        }
    }
}
