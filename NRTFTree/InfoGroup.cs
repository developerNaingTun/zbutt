/********************************************************************************
 *   This file is part of NRtfTree.
 *
 *   NRtfTree is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   NRtfTree is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with NRtfTree; if not, write to the Free Software
 *   Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 ********************************************************************************/	

/********************************************************************************
 * Library:		NRtfTree
 * Version:     v0.2.1
 * Class:		ImageNode
 * Copyright:   2005 Salvador Gomez
 * Home Page:	http://www.sgoliver.net
 * SF Project:	http://nrtftree.sourceforge.net
 *				http://sourceforge.net/projects/nrtftree
 * Date:		10/12/2006
 * Description:	Clase para encapsular toda la informaci�n contenida en un
 *              grupo RTF de tipo "\info".
 * ******************************************************************************/

using System;
using System.Text;

namespace Net.Sgoliver.NRtfTree
{
    namespace Util
    {
        /// <summary>
        /// Clase que encapsula toda la informaci�n contenida en un grupo "\info" de un documento RTF.
        /// </summary>
        public class InfoGroup
        {
            #region Atributos privados

            private string _title = "";
            private string _subject = "";
            private string _author = "";
            private string _manager = "";
            private string _company = "";
            private string _operator = "";
            private string _category = "";
            private string _keywords = "";
            private string _comment = "";
            private string _doccomm = "";
            private string _hlinkbase = "";
            private DateTime _creatim = DateTime.MinValue;
            private DateTime _revtim = DateTime.MinValue;
            private DateTime _printim = DateTime.MinValue;
            private DateTime _buptim = DateTime.MinValue;
            private int _version = -1;
            private int _vern = -1;
            private int _edmins = -1;
            private int _nofpages = -1;
            private int _nofwords = -1;
            private int _nofchars = -1;
            private int _id = -1;

            #endregion

            #region Propiedades

            /// <summary>
            /// T�tulo del documento.
            /// </summary>
            public string Title
            {
                get { return _title; }
                set { _title = value; }
            }
            
            /// <summary>
            /// Tema del documento.
            /// </summary>
            public string Subject
            {
                get { return _subject; }
                set { _subject = value; }
            }

            /// <summary>
            /// Autor del documento.
            /// </summary>
            public string Author
            {
                get { return _author; }
                set { _author = value; }
            }
            
            /// <summary>
            /// Manager del autor del documento.
            /// </summary>
            public string Manager
            {
                get { return _manager; }
                set { _manager = value; }
            }
            
            /// <summary>
            /// Compa��a del autor del documento.
            /// </summary>
            public string Company
            {
                get { return _company; }
                set { _company = value; }
            }
           
            /// <summary>
            /// �ltima persona que ha realizao cambios sobre el documento.
            /// </summary>
            public string Operator
            {
                get { return _operator; }
                set { _operator = value; }
            }
            
            /// <summary>
            /// Categor�a del documento.
            /// </summary>
            public string Category
            {
                get { return _category; }
                set { _category = value; }
            }
            
            /// <summary>
            /// Palabras clave del documento.
            /// </summary>
            public string Keywords
            {
                get { return _keywords; }
                set { _keywords = value; }
            }
            
            /// <summary>
            /// Comentarios.
            /// </summary>
            public string Comment
            {
                get { return _comment; }
                set { _comment = value; }
            }
            
            /// <summary>
            /// Comentarios mostrados en el cuadro de Texto "Summary Info" o "Properties" de Microsoft Word.
            /// </summary>
            public string DocComment
            {
                get { return _doccomm; }
                set { _doccomm = value; }
            }
            
            /// <summary>
            /// La direcci�n base usada en las rutas relativas de los enlaces del documento. Puede ser una ruta local o una URL.
            /// </summary>
            public string HlinkBase
            {
                get { return _hlinkbase; }
                set { _hlinkbase = value; }
            }
            
            /// <summary>
            /// Fecha/Hora de creaci�n del documento.
            /// </summary>
            public DateTime CreationTime
            {
                get { return _creatim; }
                set { _creatim = value; }
            }
            
            /// <summary>
            /// Fecha/Hora de revisi�n del documento.
            /// </summary>
            public DateTime RevisionTime  
            {
                get { return _revtim; }
                set { _revtim = value; }
            }
            
            /// <summary>
            /// Fecha/Hora de �ltima impresi�n del documento.
            /// </summary>
            public DateTime LastPrintTime
            {
                get { return _printim; }
                set { _printim = value; }
            }
            
            /// <summary>
            /// Fecha/Hora de �ltima copia del documento.
            /// </summary>
            public DateTime BackupTime
            {
                get { return _buptim; }
                set { _buptim = value; }
            }
            
            /// <summary>
            /// Versi�n del documento.
            /// </summary>
            public int Version
            {
                get { return _version; }
                set { _version = value; }
            }
            
            /// <summary>
            /// Versi�n interna del documento.
            /// </summary>
            public int InternalVersion
            {
                get { return _vern; }
                set { _vern = value; }
            }
            
            /// <summary>
            /// Tiempo total de edici�n del documento (en minutos).
            /// </summary>
            public int EditingTime
            {
                get { return _edmins; }
                set { _edmins = value; }
            }
            
            /// <summary>
            /// N�mero de p�ginas del documento.
            /// </summary>
            public int NumberOfPages
            {
                get { return _nofpages; }
                set { _nofpages = value; }
            }
            
            /// <summary>
            /// N�mero de palabras del documento.
            /// </summary>
            public int NumberOfWords
            {
                get { return _nofwords; }
                set { _nofwords = value; }
            }
            
            /// <summary>
            /// N�mero de caracteres del documento.
            /// </summary>
            public int NumberOfChars
            {
                get { return _nofchars; }
                set { _nofchars = value; }
            }
            
            /// <summary>
            /// Identificaci�n interna del documento.
            /// </summary>
            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

            #endregion
        }
    }
}
