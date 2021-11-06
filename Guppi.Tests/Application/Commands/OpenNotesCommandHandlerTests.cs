using System;
using FluentAssertions;
using Guppi.Application.Commands.Notes;
using NUnit.Framework;

namespace Guppi.Tests.Application.Commands
{
    [TestFixture]
    public class OpenNotesCommandHandlerTests
    {
        private string _notesDirectory;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _notesDirectory = OperatingSystem.IsWindows() ? @"C:\Notes" : "/notes";
        }

        [TestCase("2021-11-05", "Test Title", "2021-11-05 - Test Title.md" )]
        [TestCase("2021-11-05", "", "2021-11-05.md" )]
        [TestCase("2021-11-05.md", "Test Title", "2021-11-05 - Test Title.md" )]
        [TestCase("2021-11-05.md", "", "2021-11-05.md" )]
        [TestCase("2021-11-05.txt", "Test Title", "2021-11-05 - Test Title.txt" )]
        [TestCase("2021-11-05.txt", "", "2021-11-05.txt" )]
        public void TestGetFileName(string filename, string title, string expected)
        {
            var result = OpenNotesCommandHandler.GetFilename(filename, title, _notesDirectory);
            result.Should().EndWith(expected);
        }
    }
}
