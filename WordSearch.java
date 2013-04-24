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
	class FoundWord {
		String word;
		int x;
		int y;
		int direction;
		public FoundWord(String w, int r, int c, int dir) {
			word = w;
			x = r;
			y = c;
			direction = dir;
		}	
	}
	public WordSearch() {
		Scanner scanner = null;
		rowList = new ArrayList<String>();
		toFind = new ArrayList<String>();
		foundWords = new ArrayList<FoundWord>();
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
				rowList.add(line);
			}
			else if (lineCounter > startRowForWords) {
				toFind.add(line.toLowerCase());
			}
			lineCounter++;
		}
		for (String s : rowList) {
			System.out.println(s);
		}
		for (String s : toFind) {
			System.out.println(s);
		}
		for (String s : toFind) {
			System.out.println("Looking for " + s);
			findWord(s);
		}
		System.out.println("Found: ");
		for (FoundWord f : foundWords) {
			System.out.println(f.word);
		}
	}
	public static void main(String[] args) {
		WordSearch ws = new WordSearch();
	}
	public boolean findWord(String findThis) {
		int column = 0;
		int row = 0;
		boolean down = false;
		boolean diag = false;
		boolean right = false;
		while(row < numRows) {
			System.out.println("Checking row " + row);
			while (column < numColumns) {
				System.out.println("Checking column " + column);
				if (rowList.get(row).charAt(column) == findThis.charAt(0)) {
					System.out.println("Match for " + findThis.charAt(0));
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
					else
						column++;
				}
				else
					column++;
			}
			row++;
			column = 0;
		}
		return false;
	}
	public boolean searchNext(int direction, int currentRow, int currentColumn, int charPos, String findThis) {
		if (direction==1) {  //down
			currentRow+=1;
		}
		else if (direction==2) { //down-right
			currentRow+=1;
			currentColumn+=1;
		}
		else if (direction==3) { //right
			currentColumn+=1;
		}
		if (currentRow == numRows || currentColumn == numColumns) //check if OOB
			return false;
		if (rowList.get(currentRow).charAt(currentColumn)==findThis.charAt(charPos)) {
			if (charPos == findThis.length()-1) //matched all letters, obob
				return true; //found
			else
				return searchNext(direction, currentRow, currentColumn, charPos+1, findThis);
		}
		return false;
	}
}