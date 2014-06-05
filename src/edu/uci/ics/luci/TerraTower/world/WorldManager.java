/*
	Copyright 2014
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
package edu.uci.ics.luci.TerraTower.world;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import edu.uci.ics.luci.TerraTower.PasswordUtils;
import edu.uci.ics.luci.TerraTower.gameElements.Bomb;
import edu.uci.ics.luci.TerraTower.gameElements.Player;
import edu.uci.ics.luci.TerraTower.gameElements.Tower;

public class WorldManager {
	
	private static transient volatile Logger log = null;
	public static Logger getLog(){
		if(log == null){
			log = LogManager.getLogger(WorldManager.class);
		}
		return log;
	}
	
	
	//World password 
	byte[] worldHashedPassword;
	Territory territory = null;
	
	//Map from playerName to player
	HashMap<String,Player> players;
	
	List<Tower> towers;
	List<Bomb> bombs;

	
	public WorldManager(String password){
		this(PasswordUtils.hashPassword(password));
	}
	
	public WorldManager(byte[] hashedPassword){
		this.worldHashedPassword=Arrays.copyOf(hashedPassword,hashedPassword.length);
		
		players = new HashMap<String,Player>();
	
		towers = new ArrayList<Tower>();
		bombs = new ArrayList<Bomb>();
	}
	
	
	public boolean passwordGood(String proposedPassword) {
		return(PasswordUtils.checkPassword(proposedPassword,worldHashedPassword));
	}

	public boolean passwordGood(byte[] proposedPassword) {
		return(PasswordUtils.checkPassword(proposedPassword,worldHashedPassword));
	}

	public void setTerritory(Territory t) {
		territory=t;
	}
	
	public Territory getTerritory(){
		return territory;
	}

	public boolean playerExists(String playerName) {
		Player player = players.get(playerName);
		return(player!=null);
	}

	public Player createPlayer(String playerName, byte[] hashedPassword) {
		if(playerExists(playerName)){
			return null;
		}
		Player player = new Player(playerName,hashedPassword);
		players.put(playerName, player);
		return player;
	}
	
	public Player getPlayer(String playerName,String password){
		Player player= players.get(playerName);
		if(player == null){
			return null;
		}
		if(!player.passwordGood(password)){
			return null;
		}
		return(player);
	}
	
	public Player getPlayer(String playerName,byte[] hashedPassword){
		Player player= players.get(playerName);
		if(player == null){
			return null;
		}
		if(!player.passwordGood(hashedPassword)){
			return null;
		}
		return(player);
	}
	
	public boolean towerPresent(int x,int y){
		return(territory.towerPresent(x,y));
	}
	
	public boolean addTower(Tower tower){
		if(towerPresent(tower.getX(),tower.getY())){
			return false;
		}
		if(territory.addTower(tower)){
			towers.add(tower);
			return true;
		}
		else{
			return false;
		}
		
	}

	public void stepTowerTerritoryGrowth() {
		getTerritory().stepTowerTerritoryGrowth(10,2);
	}
	
	

	public int numBombsPresent(int x,int y){
		return(territory.numBombsPresent(x,y));
	}
	
	public boolean addBomb(Bomb bomb){
		return(territory.addBomb(bomb));
	}
	

	public void burnBombFuse(long eventTime) {
		getTerritory().burnBombFuse(eventTime);
	}
	
}
