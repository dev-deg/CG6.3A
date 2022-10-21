using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetData
{
    public enum CURRENCY
    {
        Default,
        Diamonds,
        Gold
    };
    
    private int _id;
    private string _name;
    private string _thumbnailUrl;
    private float _price;
    private CURRENCY _currency;

    public AssetData(int id, string name, string thumbnailUrl, float price, CURRENCY currency)
    {
        _id = id;
        _name = name;
        _thumbnailUrl = thumbnailUrl;
        _price = price;
        _currency = currency;
    }

    public virtual int ID
    {
        get => _id;
        set => _id = value;
    }

    public virtual string Name
    {
        get => _name;
        set => _name = value;
    }

    public virtual string ThumbnailUrl
    {
        get => _thumbnailUrl;
        set => _thumbnailUrl = value;
    }

    public virtual float Price
    {
        get => _price;
        set => _price = value;
    }

    public virtual CURRENCY Currency
    {
        get => _currency;
        set => _currency = value;
    }

    public override string ToString()
    {
        return ($"The asset has the following values: ID:{ID}, NAME:{Name}, URL:{ThumbnailUrl}, " +
                $"PRICE:{Price}, Currency:{Currency.ToString()}");
    }
}
