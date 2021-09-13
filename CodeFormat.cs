using System;

//Enums skriv varje i egen fil
public enum states
{
	Walk,
	Run,
	Jump
}

public class Class1
{
	// skriv ut private o public på variabler

	// variable naming
	// private camelCase
	// beskrivande
	private int exempelInt; // ex
	
	// för unity 
	[SerializedField] int health = 100;
	[SerializedField] string exempelSträng = "";

	// enkla gets på detta sätt (set är alltid en funktion)
	public int exempelProperty { get { return exempelInt; }; set }

	// const = ALLCAPS 
	private const int EXEMPELCONST;

	// Inga var om inte absolut nödvändigt!

	// function format
	// PascalCase
	// Radbryt innan måsvinge
	// skriv ut private  o public innan functionen
	public void ExempelMetod1()
	{

	}

	private void ExempelMetod2()
	{

	}

	// if- statements
	private void ExempelIfStatement()
	{
		// om vi inte ska göra något ifall bool inte är sann/ är sann.
		if (isJumping)
		{
			return;
		}

		// undvik pyramid of death och långa if-satser bryt up i flera om möjligt.

		if (canJump)
		{

		}
		else
		{

		}

	}

	// parameters namn: tydliga, gör inget om de heter samma som klassen då vi använder this. isåfall
	public void ExempelMetod3(int exempelInt)
	{
		this.exempelInt = exempelInt;
	}


	// använd denna för att sätta värdet gör inte variablen public.
	public void SetExempelInt(int value)
	{
		exempelInt = value;
		// här gör vi allting som sätter värdet på variabeln korvMedBröd för användning i propertien.
	}
	// bools
	// gör dem beskrivande 
	bool isJumping; // gör du något så är det is
	bool canJump; // kan du så är det can

	// functions namn beskrivande o korta 
	// ska bara göra en sak
	// max 4 parametrar in.
	// om funktionen anropar en massa andra funktioner relaterade till det så döp den till handle[vad funktionen hanterar]
	public void HandleMetodExempel(int scoreIncrease)
	{
		IncreaseScore();
		UpdateScore();
		PlayScoreEffects();
		//etc...
	}

	// singletons format
	public static SingletonExempel Instance { get => return instance; }

	private static SingletonExempel instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// interface naming
	// stort I i början
	// beskrivande namn
	public interface IAmExample1
	public interface IAmExample2

	public static delegate OnDelegateCall();
	public void DelegateExample(OnDelegateCall onDelegateCall)
    {

    }

}

