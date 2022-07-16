using Chatroom.Bot;
using Chatroom.Bot.Class;
using Chatroom.Bot.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Chatroom.Bot.UnitTest
{
    public class RetrieveCSVFromAPI_Test
    {
        // HP = Happy Path
        // NS = Negative Scenario

        [Fact]
        public void HP_GivenValidAPICall_WhenRetrieveCSVFromAPI_ThenReturnValidCSV()
        {            
            var mockAPIIntegration = RetrieveMockedAPIIntegrationWithValidAPICall();

            //Given
            var stockCode = "AAPL.US";

            //When
            var processStockPrice = new ProcessStockPrice(mockAPIIntegration);
            var returnedCSV = processStockPrice.RetrieveCSVFromAPI(stockCode);

            //Then
            List<string> expectedLines = new List<string>();
            expectedLines.Add("Symbol,Date,Time,Open,High,Low,Close,Volume");
            expectedLines.Add("AAPL.US,2022-07-14,21:36:53,144.08,148.95,143.25,123.45,53220265");

            AssertCSV(returnedCSV, expectedLines.ToArray());
        }

        [Fact]
        public void NS_GiveInvalidAPICall_WhenRetrieveCSVFromAPI_ThenThrowError()
        {
            var mockAPIIntegration = RetrieveMockedAPIIntegrationWithInvalidAPICall();

            //Given
            var stockCode = "META.US";

            //When
            var processStockPrice = new ProcessStockPrice(mockAPIIntegration);
            var exception = Assert.Throws<Exception>(() => processStockPrice.RetrieveCSVFromAPI(stockCode));

            //Then
            exception.Message.Should().Be("404 (URL not found)");
        }

        [Fact]
        public void NS_GiveValidAPICallThatReturnsEmptyContent_WhenRetrieveCSVFromAPI_ThenThrowError()
        {
            var mockAPIIntegration = RetrieveMockedAPIIntegrationWithEmptyContent();

            //Given
            var stockCode = "VT.US";

            //When
            var processStockPrice = new ProcessStockPrice(mockAPIIntegration);
            var exception = Assert.Throws<Exception>(() => processStockPrice.RetrieveCSVFromAPI(stockCode));

            //Then
            exception.Message.Should().Be(String.Format("Empty data retrieved for stockcode {0}.", stockCode));
        }

        #region Util
        private void AssertCSV(StreamReader csvToAssert, string[] expectedLines)
        {
            string line;

            // First Line
            line = csvToAssert.ReadLine();
            Assert.True(line == expectedLines[0], "Received header from CSV does not match with expected header.");

            // Second Line
            line = csvToAssert.ReadLine();
            Assert.True(line == expectedLines[1], "Received values from CSV does not match with expected values.");
        }

        private IAPIIntegration RetrieveMockedAPIIntegrationWithValidAPICall()
        {
            var mockAPIIntegration = new Mock<IAPIIntegration>();

            using (FileStream validCSV = new FileStream(@".\MockCSVs\ValidCSV.csv", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                MemoryStream memoryStream = new MemoryStream();                
                validCSV.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var returnMessage = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(memoryStream)
                };

                validCSV.Close();

                mockAPIIntegration.Setup(psp => psp.CallAPI(It.IsAny<string>())).Returns(returnMessage);
            }

            return mockAPIIntegration.Object;
        }

        private IAPIIntegration RetrieveMockedAPIIntegrationWithInvalidAPICall()
        {
            var mockAPIIntegration = new Mock<IAPIIntegration>();

            var returnMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound,
                ReasonPhrase = "URL not found"
            };

            mockAPIIntegration.Setup(psp => psp.CallAPI(It.IsAny<string>())).Returns(returnMessage);

            return mockAPIIntegration.Object;
        }

        private IAPIIntegration RetrieveMockedAPIIntegrationWithEmptyContent()
        {
            var mockAPIIntegration = new Mock<IAPIIntegration>();

            var returnMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(Stream.Null)
            };

            mockAPIIntegration.Setup(psp => psp.CallAPI(It.IsAny<string>())).Returns(returnMessage);

            return mockAPIIntegration.Object;
        }
        #endregion
    }
}
