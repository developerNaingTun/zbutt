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
 * Description:	Token leido por el analizador l�xico para documentos RTF.
 * ******************************************************************************/

using System;

namespace Net.Sgoliver.NRtfTree
{
    namespace Core
    {
        /// <summary>
        /// Token leido por el analizador l�xico para documentos RTF.
        /// </summary>
        public class RtfToken
        {
            #region Atributos P�blicos

            /// <summary>
            /// Tipo del token.
            /// </summary>
            private RtfTokenType type;
            /// <summary>
            /// Palabra clave / S�mbolo de Control / Caracter.
            /// </summary>
            private string key;
            /// <summary>
            /// Indica si el token tiene par�metro asociado.
            /// </summary>
            private bool hasParam;
            /// <summary>
            /// Par�metro de la palabra clave o s�mbolo de Control.
            /// </summary>
            private int param;

            #endregion

			#region Propiedades

			/// <summary>
			/// Tipo del token.
			/// </summary>
			public RtfTokenType Type
			{
				get
				{
					return type;
				}
				set
				{
					type = value;
				}
			}

			/// <summary>
			/// Palabra clave / S�mbolo de Control / Caracter.
			/// </summary>
			public string Key
			{
				get
				{
					return key;
				}
				set 
				{
					key = value;
				}
			}

			/// <summary>
			/// Indica si el token tiene par�metro asociado.
			/// </summary>
			public bool HasParameter
			{
				get
				{
					return hasParam;
				}
				set
				{
					hasParam = value;
				}
			}

			/// <summary>
			/// Par�metro de la palabra clave o s�mbolo de Control.
			/// </summary>
			public int Parameter
			{
				get
				{
					return param;
				}
				set
				{
					param = value;
				}
			}

			#endregion

            #region Constructor P�blico

            /// <summary>
            /// Constructor de la clase RtfToken. Crea un token vac�o.
            /// </summary>
            public RtfToken()
            {
                type = RtfTokenType.None;
                key = "";
                
				/* Inicializados por defecto */
				//hasParam = false;
				//param = 0;
            }

            #endregion
        }
    }
}
