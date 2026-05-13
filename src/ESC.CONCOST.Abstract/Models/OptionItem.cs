using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ESC.CONCOST.Abstract;

public class OptionItem<T> 
{
    public OptionItem()
    {
    }

    public OptionItem(T value, string text)
    {
        Value = value;
        Text = text;
    }
    
    public T Value { get; set; }

    public T[] Values { get; set; }

    public string Text { get; set; }    

    public DateTime DateCreate { get; set; }

    public string User { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string PositionName { get; set; }

    public string BreakType { get; set; }

    public string BreakTime { get; set; }

    public dynamic Attributes { get; set; }




    //
    public string Avatar { get; set; }

    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string Summary { get; set; }

    public string Phone { get; set; }

    public string WebSite { get; set; }

    public string Email { get; set; }

    public string Gender { get; set; }

    public string Address { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public Guid _OfficeGuid { get; set; }

    public Guid EmployeeGuid { get; set; }

    public bool IsPrimary { get; set; }

    public bool Apply { get; set; }

    public int Id { get; set; }

    public string CCCD { get; set; }

}

public class OptionItems<T> : List<OptionItem<T>>
{
    private Dictionary<T, int> _dict = new();
    public OptionItems(params OptionItem<T>[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            this.Add(items[i]);
            _dict.Add(items[i].Value, i);          
        }
    }
    
    public OptionItem<T> this[T value]
    {
        get
        {
            if (value != null && _dict.ContainsKey(value))
            {
                return this[_dict[value]];
            }
            else
            {
                return new OptionItem<T>(default(T), string.Empty);
            }
        }
    } 

    public List<OptionItem<T>> GetFilters(string option)
    {
        var list = new List<OptionItem<T>>();
        list.Add(new(default(T), option));
        list.AddRange(this);
        return list;
    }
}

public static class OptionItemUtil
{
    public static List<OptionItem<T>> GetFilters<T>(this List<OptionItem<T>> list, string option)
    {
        var newList = new List<OptionItem<T>>();
        newList.Add(new(default(T), option));
        newList.AddRange(list);
        return newList;
    }

    public static OptionItems<T> Excepts<T>(this List<OptionItem<T>> options, params T[] excepts)
    {
        return new OptionItems<T>(options.Where(x => !excepts.Any(y => y.Equals(x.Value))).ToArray());
    }
}