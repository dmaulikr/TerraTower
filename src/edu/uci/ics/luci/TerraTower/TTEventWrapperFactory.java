/*
	Copyright 2014-2015
		University of California, Irvine (c/o Donald J. Patterson)
*/
/*
	This file is part of the Laboratory for Ubiquitous Computing java TerraTower game, i.e. "TerraTower"

    TerraTower is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Utilities is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Utilities.  If not, see <http://www.gnu.org/licenses/>.
*/
package edu.uci.ics.luci.TerraTower;

import com.lmax.disruptor.EventFactory;

import edu.uci.ics.luci.TerraTower.events.TTEventType;

public class TTEventWrapperFactory implements EventFactory<TTEventWrapper>
{
    TTEventType defaultEventType = TTEventType.VOID;
	
	TTEventWrapperFactory(){
	}
	
	TTEventWrapperFactory(TTEventType d){
		this.defaultEventType = d;
	}
	
	
    public TTEventWrapper newInstance()
    {
        return new TTEventWrapper(defaultEventType,null,(TTEventHandlerResultListener)null);
    }
}
