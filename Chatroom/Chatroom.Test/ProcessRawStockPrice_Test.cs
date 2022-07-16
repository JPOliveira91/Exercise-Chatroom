using Chatroom.Bot;
using FluentAssertions;
using System;
using Xunit;

namespace Chatroom.Test
{
    public class ProcessRawStockPrice_Test
    {
        // HP = Happy Path
        // NS = Negative Scenario

        [Fact]
        public void HP_GivenValidRawStockPriceWithTwoDecimalPlaces_WhenProcessRawStockPrice_ThenReturnStockPrice()
        {
            //Given
            var stockCode = "AAPL.US";
            string rawStockPrice = "987.65";

            //When
            var processStockPrice = new ProcessStockPrice();
            var returnedStockPrice = processStockPrice.ProcessRawStockPrice(stockCode, rawStockPrice);

            //Then
            string expectedStockPrice = "987.65";
            returnedStockPrice.Should().Be(expectedStockPrice);
        }

        [Fact]
        public void HP_GivenValidRawStockPriceWithLessThanTwoDecimalPlaces_WhenProcessRawStockPrice_ThenReturnStockPrice()
        {
            //Given
            var stockCode = "AAPL.US";
            string rawStockPrice = "54";

            //When
            var processStockPrice = new ProcessStockPrice();
            var returnedStockPrice = processStockPrice.ProcessRawStockPrice(stockCode, rawStockPrice);

            //Then
            string expectedStockPrice = "54.00";
            returnedStockPrice.Should().Be(expectedStockPrice);
        }

        [Fact]
        public void HP_GivenValidRawStockPriceWithMoreThanTwoDecimalPlaces_WhenProcessRawStockPrice_ThenReturnStockPrice()
        {
            //Given
            var stockCode = "AAPL.US";
            string rawStockPrice = "159.35715534";

            //When
            var processStockPrice = new ProcessStockPrice();
            var returnedStockPrice = processStockPrice.ProcessRawStockPrice(stockCode, rawStockPrice);

            //Then
            string expectedStockPrice = "159.36";
            returnedStockPrice.Should().Be(expectedStockPrice);
        }

        [Fact]
        public void NS_GivenInvalidRawStockPrice_WhenProcessRawStockPrice_ThenThrowError()
        {
            //Given
            var stockCode = "AAPL.US";
            string rawStockPrice = "ABCDEF";

            //When
            var processStockPrice = new ProcessStockPrice();
            var exception = Assert.Throws<Exception>(() => processStockPrice.ProcessRawStockPrice(stockCode, rawStockPrice));

            //Then
            exception.Message.Should().Be(String.Format("Invalid price in retrieved CSV for stockcode {0}.", stockCode));
        }

        [Fact]
        public void NS_GivenEmptyRawStockPrice_WhenProcessRawStockPrice_ThenThrowError()
        {
            //Given
            var stockCode = "AAPL.US";
            string rawStockPrice = "";

            //When
            var processStockPrice = new ProcessStockPrice();
            var exception = Assert.Throws<Exception>(() => processStockPrice.ProcessRawStockPrice(stockCode, rawStockPrice));

            //Then
            exception.Message.Should().Be(String.Format("Invalid price in retrieved CSV for stockcode {0}.", stockCode));
        }
    }
}
