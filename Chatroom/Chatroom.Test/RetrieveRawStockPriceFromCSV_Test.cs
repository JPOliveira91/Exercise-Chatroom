using Chatroom.Bot;
using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace Chatroom.Test
{
    public class RetrieveRawStockPriceFromCSV_Test
    {
        // HP = Happy Path
        // NS = Negative Scenario

        [Fact]
        public void HP_GivenValidCSV_WhenRetrieveRawStockPriceFromCSV_ThenReturnRawStockPrice()
        {
            //Given
            var stockCode = "AAPL.US";
            var csvStream = new StreamReader(@".\MockCSVs\ValidCSV.csv");

            //When
            var processStockPrice = new ProcessStockPrice();
            var returnedRawStockPrice = processStockPrice.RetrieveRawStockPriceFromCSV(stockCode, csvStream);

            //Then
            returnedRawStockPrice.Should().Be("123.45");
        }

        [Fact]
        public void NS_GivenCSVWithoutValues_WhenRetrieveRawStockPriceFromCSV_ThenThrowError()
        {
            //Given
            var stockCode = "AAPL.US";
            var csvStream = new StreamReader(@".\MockCSVs\CSVWithoutValues.csv");

            //When
            var processStockPrice = new ProcessStockPrice();
            var exception = Assert.Throws<Exception>(() => processStockPrice.RetrieveRawStockPriceFromCSV(stockCode, csvStream));

            //Then
            exception.Message.Should().Be(String.Format("Error while retrieving price in CSV for stock {0}.", stockCode));
        }

        [Fact]
        public void NS_GivenEmptyCSV_WhenRetrieveRawStockPriceFromCSV_ThenThrowError()
        {
            //Given
            var stockCode = "META.US";
            var csvStream = new StreamReader(@".\MockCSVs\EmptyCSV.csv");

            //When
            var processStockPrice = new ProcessStockPrice();
            var exception = Assert.Throws<Exception>(() => processStockPrice.RetrieveRawStockPriceFromCSV(stockCode, csvStream));

            //Then
            exception.Message.Should().Be(String.Format("Error while retrieving price in CSV for stock {0}.", stockCode));
        }
    }
}
