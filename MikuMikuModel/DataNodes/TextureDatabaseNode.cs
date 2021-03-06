﻿using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    public class TextureDatabaseNode : BinaryFileNode<TextureDatabase>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags
        {
            get
            {
                return
                  DataNodeActionFlags.Export | DataNodeActionFlags.Move |
                  DataNodeActionFlags.Remove | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;
            }
        }

        [Browsable( false )]
        public ListNode<TextureEntry> Textures { get; set; }

        protected override void InitializeCore()
        {
            RegisterExportHandler<TextureDatabase>( ( path ) =>
            {
                // Assume it's being exported for F2nd PS3
                if ( BinaryFormatUtilities.IsClassic( Data.Format ) && path.EndsWith( ".txi", StringComparison.OrdinalIgnoreCase ) )
                {
                    Data.Format = BinaryFormat.F2nd;
                    Data.Endianness = Endianness.BigEndian;
                }

                // Or reverse
                else if ( BinaryFormatUtilities.IsModern( Data.Format ) && path.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
                {
                    Data.Format = BinaryFormat.DT;
                    Data.Endianness = Endianness.LittleEndian;
                }

                Data.Save( path );
            } );
            RegisterReplaceHandler<TextureDatabase>( BinaryFile.Load<TextureDatabase> );
            RegisterDataUpdateHandler( () =>
            {
                var data = new TextureDatabase();
                data.Format = Format;
                data.Endianness = Endianness;
                data.Textures.AddRange( Textures.Data );
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( Textures = new ListNode<TextureEntry>( "Textures", Data.Textures ) );
        }

        protected override void OnReplace( object oldData )
        {
            TextureDatabase oldDataT = ( TextureDatabase )oldData;

            // Pass the format/endianness
            Data.Format = oldDataT.Format;
            Data.Endianness = oldDataT.Endianness;
        }

        public TextureDatabaseNode( string name, TextureDatabase data ) : base( name, data )
        {
        }
    }

    public class TextureEntryNode : DataNode<TextureEntry>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags =>
            DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename;

        public int ID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Name ) );
            base.OnRename( oldName );
        }

        public TextureEntryNode( string name, TextureEntry data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
