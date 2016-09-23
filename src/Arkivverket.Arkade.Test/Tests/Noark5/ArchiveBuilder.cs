using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Arkivverket.Arkade.ExternalModels.Noark5;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ArchiveBuilder : IKanLeggeTilKlassifikasjonssystem, IKanLeggeTilKlasse, IKanLeggeTilMappe
    {

        private arkiv _arkiv;
        private arkivdel _arkivdel;
        private klassifikasjonssystem _klassifikasjonssystem;
        private klasse _klasse;
        private mappe _mappe;
        private arkiv _underarkiv;

        private arkiv _valgtArkiv;

        private ArchiveBuilder()
        {
            _arkiv = new arkiv();
            _valgtArkiv = _arkiv;
        }

        public static IKanLeggeTilArkivdelEllerUnderarkiv Arkiv()
        {
            return new ArchiveBuilder();
        }

        public IKanLeggeTilKlassifikasjonssystem Arkivdel()
        {
            _arkivdel = new arkivdel();

            _valgtArkiv.Items = AppendOrCreateNewArray(_valgtArkiv.Items, _arkivdel);
            return this;
        }

        public IKanLeggeTilArkivdelEllerUnderarkiv Underarkiv()
        {
            _underarkiv = new arkiv();
            _valgtArkiv.Items = AppendOrCreateNewArray(_valgtArkiv.Items, _underarkiv);
            _valgtArkiv = _underarkiv;
            return this;
        }

        object[] AppendOrCreateNewArray(object[] existingObjects, object newObject)
        {
            List<object> items = new List<object>();
            if (existingObjects != null)
                items.AddRange(existingObjects);

            items.Add(newObject);

            return items.ToArray();
        }

        T[] AppendOrCreateNewArray<T>(T[] existingObjects, T newObject)
        {
            List<T> items = new List<T>();
            if (existingObjects != null)
                items.AddRange(existingObjects);

            items.Add(newObject);

            return items.ToArray();
        }

        public IKanLeggeTilKlasse Klassifikasjonssystem()
        {
            _klassifikasjonssystem = new klassifikasjonssystem();
            _arkivdel.Items = AppendOrCreateNewArray(_arkivdel.Items, _klassifikasjonssystem);
            return this;
        }

        public IKanLeggeTilMappe Klasse()
        {
            _klasse = new klasse();
            _klassifikasjonssystem.klasse = AppendOrCreateNewArray(_klassifikasjonssystem.klasse, _klasse);
            return this;
        }

        public IKanLeggeTilMappe Mappe()
        {
            _mappe = new mappe();
            _klasse.Items = AppendOrCreateNewArray(_klasse.Items, _mappe);
            return this;
        }

        public Stream Build()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add(string.Empty, "http://www.arkivverket.no/standarder/noark5/arkivstruktur");

            return SerializeUtil.SerializeToStream(_arkiv, namespaces);
        }

    }

    public interface IKanLeggeTilArkivdelEllerUnderarkiv
    {
        IKanLeggeTilKlassifikasjonssystem Arkivdel();
        IKanLeggeTilArkivdelEllerUnderarkiv Underarkiv();
        Stream Build();
    }

    public interface IKanLeggeTilKlassifikasjonssystem : IKanLeggeTilArkivdelEllerUnderarkiv
    {
        IKanLeggeTilKlasse Klassifikasjonssystem();
        new Stream Build();
    }

    public interface IKanLeggeTilKlasse : IKanLeggeTilArkivdelEllerUnderarkiv
    {
        IKanLeggeTilMappe Klasse();
        new Stream Build();
    }

    public interface IKanLeggeTilMappe : IKanLeggeTilKlasse
    {
        IKanLeggeTilMappe Mappe();
        new Stream Build();
    }

}