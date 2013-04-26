import java.util.Scanner;
import java.io.*;
import java.util.*;

public class WordSearch {
	final static int numRows = 25;
	final static int numColumns = 20;
	final static int startRowForWords = 26;
	List<String> rowList;
	List<String> toFind;
	List<FoundWord> foundWords;
	List<String> notFound;
	char[][] solution;
	class FoundWord {
		String word;
		int row;
		int column;
		int direction;
		public FoundWord(String w, int r, int c, int dir) {
			word = w;
			row = r;
			column = c;
			direction = dir;
		}	
	}
	public WordSearch() {
		Scanner scanner = null;
		rowList = new ArrayList<String>();
		toFind = new ArrayList<String>();
		solution = new char[numRows][numColumns];
		foundWords = new ArrayList<FoundWord>();
		notFound = new ArrayList<String>();
		try {
			scanner = new Scanner(new File("word-search.txt"));
		} catch(FileNotFoundException e) {
			System.out.println("Couldn't find file word-search.txt");
		}
		int lineCounter = 0;
		while(scanner.hasNextLine()) {
			Scanner lineScanner = new Scanner(scanner.nextLine()).useDelimiter("\t");
			String line = new String();
			while(lineScanner.hasNext()) {
				line = line + lineScanner.next();
			}
			lineScanner.close();
			if (lineCounter<numRows) {
				rowList.add(line); //read in puzzle to List<String>
			}
			else if (lineCounter > startRowForWords) {
				line = line.replaceAll("\\s", "");
				toFind.add(line.toLowerCase()); //read in words to find to List<String>
			}
			lineCounter++;
		}
		for (String s : rowList) {
			System.out.println(s);
		}
		System.out.println("\nWords to find: ");
		for (String s : toFind) {
			System.out.println(s);
		}
		for (String s : toFind) {
			findWord(s);
		}
		for (int i=0; i<numRows; i++) { //create blank solution grid 
			for (int j=0; j<numColumns; j++) {
				solution[i][j] = ' ';
			}
		}
		for (FoundWord f : foundWords) { //fill in solution grid with found words
			for (int i=0; i<f.word.length(); i++) {
				solution[f.row][f.column] = f.word.charAt(i);
				if (f.direction==1) {
					f.row++;
					f.column--;
				}
				else if (f.direction==2) {
					f.row++;
				}
				else if (f.direction==3) {
					f.row++;
					f.column++;
				}
				else if (f.direction==4) {
					f.column--;
				}
				else if (f.direction==6) {
					f.column++;
				}
				else if (f.direction==8) {
					f.row--;
				}
			}
		}
		System.out.println("\n\n");
		for(int i=0; i<numRows; i++) { //display solution grid
			for (int j=0; j<numColumns; j++) {
				System.out.print(solution[i][j]);
			}
			System.out.println("");
		}
		
		if (!notFound.isEmpty()) {
			System.out.println("\n\nNot found:");
			for(String s : notFound) {
				System.out.println(s);
			}
		}
		else {
			System.out.println("All words found");
		}
		
	}
	public boolean findWord(String findThis) {
		int column = 0;
		int row = 0;
		while(row < numRows) {
			while (column < numColumns) {
				if (rowList.get(row).charAt(column) == findThis.charAt(0)) {
					if (searchNext(1, row, column, 1, findThis)) {
						foundWords.add(new FoundWord(findThis, row, column, 1));
						System.out.println("Found " + findThis);
						return true;
					}
					else if (searchNext(2, row, column, 1, findThis)) {
						foundWords.add(new FoundWord(findThis, row, column, 2));
						System.out.println("Found " + findThis);
						return true;
					}
					else if (searchNext(3, row, column, 1, findThis)) {
						foundWords.add(new FoundWord(findThis, row, column, 3));
						System.out.println("Found " + findThis);
						return true;
					}
					else if (searchNext(4, row, column, 1, findThis)) {
						foundWords.add(new FoundWord(findThis, row, column, 4));
						System.out.println("Found " + findThis);
						return true;
					}
					else if (searchNext(6, row, column, 1, findThis)) {
						foundWords.add(new FoundWord(findThis, row, column, 6));
						System.out.println("Found " + findThis);
						return true;
					}
					else if (searchNext(8, row, column, 1, findThis)) {
						foundWords.add(new FoundWord(findThis, row, column, 8));
						System.out.println("Found " + findThis);
						return true;
					}
					else
						column++;
				}
				else
					column++;
			}
			row++;
			column = 0;
		}
		notFound.add(findThis);
		return false;
	}
	public boolean searchNext(int direction, int currentRow, int currentColumn, int charPos, String findThis) {
		if (direction==1) { //down left
			currentRow+=1;
			currentColumn-=1;
		}
		else if (direction==2) {  //down
			currentRow+=1;
		}
		else if (direction==3) { //down-right
			currentRow+=1;
			currentColumn+=1;
		}
		else if (direction==4) { //left
			currentColumn-=1;
		}
		else if (direction==6) { //right
			currentColumn+=1;
		}
		else if (direction==8) {  //up
			currentRow-=1;
		}
		
		if (currentRow == numRows || currentColumn < 0 || currentRow <0 || currentColumn == numColumns) //check if OOB
			return false;
		if (rowList.get(currentRow).charAt(currentColumn)==findThis.charAt(charPos)) { //matched a letter
			if (charPos == findThis.length()-1) //matched all letters 
				return true; //found
			else //not yet
				return searchNext(direction, currentRow, currentColumn, charPos+1, findThis); //recursive call
		}
		return false;
	}
	public static void main(String[] args) {
		WordSearch ws = new WordSearch();
	}
}