using System.Collections.Immutable;
using System.Xml.Linq;
using System.Xml.XPath;
using Galapa.Core.StreamObfuscator;

namespace Galapa.Core.Configuration;

public class PlayerListXml : ConfigFile
{
    private Dictionary<string, SavedPlayer> _players = new();

    private PlayerListXml() : base("dqxPlayerList.xml", 0x11, UsernameObfuscator.Factory)
    {
    }

    protected override string DefaultContents => """
                                                 <?xml version="1.0" encoding="UTF-8"?>
                                                 <DragonQuestX>
                                                     <PlayerList Version="0.9.0" LastSelect="0">
                                                     </PlayerList>
                                                 </DragonQuestX>
                                                 """;

    private XElement PlayerListNode => this.Document?.XPathSelectElement("//DragonQuestX/PlayerList")!;
    private XElement? TrialInfoNode => this.PlayerListNode.XPathSelectElement("//TrialInfo");
    public ImmutableDictionary<string, SavedPlayer> Players => this._players.ToImmutableDictionary();

    public TrialPlayer? Trial
    {
        get => this.TrialInfoNode is null ? null : new TrialPlayer(this.TrialInfoNode);
        set
        {
            this.TrialInfoNode?.Remove();
            this.PlayerListNode.Add(value?.Element);
        }
    }

    /// <summary>
    ///     Remove a player from the XML file.
    /// </summary>
    /// <param name="player">The player to be removed</param>
    /// <exception cref="InvalidConfigException">The config file is structured incorrectly</exception>
    public void Add(SavedPlayer player)
    {
        if (this.PlayerListNode is null) throw this.Invalid();

        this._players.Add(player.Token, player);

        if (this.TrialInfoNode is null)
            this.PlayerListNode.Add(player.Element);
        else
            this.TrialInfoNode.AddBeforeSelf(player.Element);
    }

    /// <summary>
    ///     Add a player to the XML file.
    /// </summary>
    /// <param name="player">The player info to be added</param>
    /// <exception cref="InvalidConfigException">The config file is structured incorrectly</exception>
    public void Remove(SavedPlayer player)
    {
        if (this.PlayerListNode is null) throw this.Invalid();
        this._players.Remove(player.Token);
        player.Element.Remove();
    }

    /// <summary>
    ///     Load, parse, and validate the dqxPlayerList.xml file.
    /// </summary>
    /// <exception cref="InvalidConfigException">The config file is structured incorrectly</exception>
    public static async Task<PlayerListXml> LoadAsync()
    {
        var instance = new PlayerListXml();
        await instance._LoadAsync();
        return instance;
    }

    protected override async Task _LoadAsync()
    {
        await base._LoadAsync();
        if (this.Document is null) throw this.Invalid();
        if (this.PlayerListNode is null) throw this.Invalid();
        this._players = this.PlayerListNode
            .XPathSelectElements("//Player")
            .Select(el => new SavedPlayer(el))
            .ToDictionary(p => p.Token);
    }

    public abstract class Player(XElement element)
    {
        public XElement Element { get; } = element;

        protected InvalidConfigException Invalid()
        {
            return new InvalidConfigException(this.Element.ToString());
        }
    }

    public class SavedPlayer(XElement element) : Player(element)
    {
        public SavedPlayer() : this(new XElement("Player"))
        {
        }

        public int Number
        {
            get
            {
                if (this.Element.Attribute("Number")?.Value is { } rawNumber &&
                    int.TryParse(rawNumber, out var number))
                    return number;

                throw this.Invalid();
            }
            set => this.Element.SetAttributeValue("Number", value);
        }

        public string Token
        {
            get
            {
                if (this.Element.Attribute("Token")?.Value is { } token) return token;

                throw this.Invalid();
            }
            set => this.Element.SetAttributeValue("Token", value);
        }
    }

    public class TrialPlayer(XElement element) : Player(element)
    {
        public TrialPlayer() : this(new XElement("TrialInfo"))
        {
        }

        public string Id
        {
            get
            {
                if (this.Element.Attribute("ID")?.Value is { } id) return id;

                throw this.Invalid();
            }
            set => this.Element.SetAttributeValue("ID", value);
        }

        public string Token
        {
            get
            {
                if (this.Element.Attribute("Token")?.Value is { } token) return token;

                throw this.Invalid();
            }
            set => this.Element.SetAttributeValue("Token", value);
        }

        public string Code
        {
            get
            {
                if (this.Element.Attribute("Code")?.Value is { } code) return code;

                throw this.Invalid();
            }
            set => this.Element.SetAttributeValue("Code", value);
        }
    }
}