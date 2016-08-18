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
 * Description:	Tipos de token de un árbol de documento RTF.
 * ******************************************************************************/

using System;

namespace Net.Sgoliver.NRtfTree
{
    namespace Core
    {
        /// <summary>
        /// Tipos de token de un árbol de documento RTF.
        /// </summary>
        public enum RtfTokenType
        {
            /// <summary>
            /// Indica que el token sólo se ha inicializado.
            /// </summary>
            None = 0,
            /// <summary>
            /// Palabra clave sin parámetro.
            /// </summary>
            Keyword = 1,
            /// <summary>
            /// Símbolo de Control sin parámetro.
            /// </summary>
            Control = 2,
            /// <summary>
            /// Texto del documento.
            /// </summary>
            Text = 3,
            /// <summary>
            /// Marca de fin de fichero.
            /// </summary>
            Eof = 4,
            /// <summary>
            ///	Inicio de grupo: '{'
            /// </summary>
            GroupStart = 5,
            /// <summary>
            /// Fin de grupo: '}'
            /// </summary>
            GroupEnd = 6
        }
    }
}
