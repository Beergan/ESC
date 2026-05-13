namespace ESC.CONCOST.Abstract;

public interface IEntity<TypeOfKey>
{
    TypeOfKey Id { get; set; }
}