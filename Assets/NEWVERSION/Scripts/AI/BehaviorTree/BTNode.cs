// Gốc của mọi node trong cây hành vi
public abstract class BTNode
{
    public enum State { Running, Success, Failure }
    public abstract State Evaluate();
}