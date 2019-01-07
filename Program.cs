using System;
using static System.Console;

using Game;

namespace blackjack
{	
    class Program
    {
        static void Main(string[] args)
        {
			BlackJackGame myGame = new BlackJackGame( );
			
			WriteLine( "Welcome to BlackJack! Each player will start with 100 coins. Increase your wealth by beating the dealer!" );
			
			while( true )
			{
				Write( "Enter number of players: " );
				string response = ReadLine( );
				WriteLine( );
				
				if( int.TryParse( response, out int val ) )
				{
					if( val >= 1 && val <= 8 )
					{
						myGame.NumberOfPlayers = val;
						break;
					}
				}
				Write( "Players should be between 1 and 8. (inclusive) " );
			}
			
			myGame.CreatePlayers( );
			bool wantsToPlay = true;
			
			int numberOfRounds = 1;
			
			do
			{
				WriteLine( $"This is round {numberOfRounds}." );
			
				myGame.CollectBets( );
			
				WriteLine( "Dealing the first card " );
				myGame.DealRound( );
				
				WriteLine( "Dealing the second card " );
				myGame.DealRound( );
				
				myGame.MainRound( );
				
				myGame.DealerRound( );
				
				myGame.CompareToDealer( );
				
				myGame.EndRound( );
				
				if( !myGame.CheckGame( ) ) 
				{
					WriteLine( "All players are out!!" );
					break; 
				}
				
				myGame.ResetDeck( );
				WriteLine( "Enter 'stop' if you would like to stop playing. Otherwise, enter anything else." );
				wantsToPlay = ReadLine( ).Equals( "stop" ) ? false : true ;
				
				numberOfRounds++;
			}
			while( wantsToPlay );
		
			myGame.EndGame( ); 
        }
    }
}
