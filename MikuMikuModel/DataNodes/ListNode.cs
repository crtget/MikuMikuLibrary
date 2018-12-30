﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MikuMikuModel.DataNodes
{
    public class ListNode<T> : DataNode<List<T>> where T : class
    {
        private Type type;
        private Func<T, string> nameGetter;

        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.None; }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.Folder; }
        }

        public int Count => GetProperty<int>();

        protected override void InitializeCore()
        {
            if ( !DataNodeFactory.DataNodeTypes.TryGetValue( typeof( T ), out type ) )
                throw new KeyNotFoundException( "Given type for the list could not be found in data node library" );

            RegisterDataUpdateHandler( () => Nodes.Select( x => ( T )x.Data ).ToList() );
        }

        protected override void InitializeViewCore()
        {
            for ( int i = 0; i < Data.Count; i++ )
            {
                var name = nameGetter != null ? nameGetter( Data[ i ] ) : $"{DataNodeFactory.GetSpecialName( type )} #{i}";
                Add( DataNodeFactory.Create<T>( name, Data[ i ] ) );
            }
        }

        public ListNode( string name, List<T> data ) : base( name, data )
        {
        }

        public ListNode( string name, List<T> data, Func<T, string> nameGetterFunc ) : base( name, data )
        {
            nameGetter = nameGetterFunc;
        }
    }
}
