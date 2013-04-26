import java.io.File;
import java.io.FileNotFoundException;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

public class ThreeLetterWord {
	 Set<String> dictionary = new HashSet<String>();
	 Scanner scanner;
	 Scanner inputScanner;
	 ConcurrentHashMap<String, Node> graph;
	 static String alphabet = "abcdefghijklmnopqrstuvwxyz";
	 class Node {
		 String word;
		 Node predecessor;
		 ArrayList<Node> nodeList;
		 boolean visited;
		 public Node(String s) {
			 word = s;
			 predecessor = null;
			 visited = false;
		 }
		 public void setPredecessor(Node n) {
			 predecessor = n;
		 }
		 public void setList(ArrayList<Node> a) {
			 nodeList = a;
		 }
		 public ArrayList<Node> getList() {
			 return nodeList;
		 }
		 public Node getPredecessor() {
			 return predecessor;
		 }
		 public void setVisited() {
			 visited = true;
		 }
		 public boolean getVisited() {
			 return visited;
		 }
	 }
	 public ThreeLetterWord() {
		try {
			scanner = new Scanner(new File("three-letter-words.txt"));
		} catch(FileNotFoundException e) {
			System.out.println("Couldn't find file three-letter-words.txt");
		}
		while (scanner.hasNext()) {
			dictionary.add(scanner.nextLine().toLowerCase()); //add all 3-letter words to dictionary
		}
		inputScanner = new Scanner(System.in);
		String start, end;
		while(true) {
			graph = new ConcurrentHashMap<String, Node>();
			System.out.println("Enter starting word: ");
			start = inputScanner.nextLine();
			System.out.println("Enter ending word: ");
			end = inputScanner.nextLine();
			makeGraph(graph, start); //build first layer of graph
			breadthfirstsearch(graph, start, end);
			System.out.println("");
		 }
	 }
	 public void breadthfirstsearch(ConcurrentHashMap<String, Node> graph, String start, String end) {
		 ArrayDeque<Node> queue = new ArrayDeque<Node>();
		 ArrayList<Node> nodeList;
		 String temp;
		 queue.add(graph.get(start)); //add first node to queue
		 looking:
		 while(!queue.isEmpty()) {
			 temp = queue.pop().word;
			 if (temp.equals(end)) { //check if this is the word we're looking for
				 queue.clear();
				 break;
			 }
			 nodeList = makeGraph(graph, temp); //if not, need to go another layer in
			 for(Node n : nodeList) {
				 if (!graph.get(n.word).getVisited()) {
					 graph.get(n.word).setPredecessor(graph.get(temp));
					 graph.get(n.word).setVisited();
					 queue.add(graph.get(n.word)); //add all of this word's neighbors to queue
					 if (n.word.equals(end)) { //if the word we want is in this word's neighbors
						 queue.clear();		//done looking
						 break looking;
					 }
				 }
			 }
		 }
		 if (graph.get(end)==null) { //no such path exists
			 System.out.println("Could not find match to " + end);
		 }
		 else {
			 System.out.print(start + " -> ");
			 String from = end;
			 int count = 1;
		     while (graph.get(from).getPredecessor()!=null) { //moving backwards from end word
		    	 if (!graph.get(from).getPredecessor().word.equals(start)) {
		    		 queue.add(graph.get(from).getPredecessor()); //push all predecessors to queue
		    	 }
		    	 else
		    		 break;
		    	 from = graph.get(from).getPredecessor().word;
		    }
		     while (!queue.isEmpty()) { //print in correct order now
		         System.out.print(queue.pollLast().word + " -> ");
		         count++;
		     }
		     System.out.print(end);
		     System.out.println("\nTook " + count + " moves");
		 }
	 }	 
	 public ArrayList<String> findNeighbors(String word) { //find all neighbors for this word
		 ArrayList<String> neighbors = new ArrayList<String>();
		 StringBuilder theWord = new StringBuilder(word);
		 StringBuilder newWord;
		 for (int i=0; i<3; i++) { //check all combinations with 1st, 2nd, 3rd letter swapped
			 for (int j=0; j<26; j++) { //check all possible letters to swap
				 if (alphabet.charAt(j)!=theWord.charAt(i)) { //check if same letter
					 newWord = new StringBuilder(word);
					 newWord.setCharAt(i, alphabet.charAt(j));
					 if (dictionary.contains(newWord.toString())) {//check if word exists
						 neighbors.add(newWord.toString()); //if so, add to neighbors
					 }
				 }
			 }
		 }
		 return neighbors;
	 }
 
	 public ArrayList<Node> makeGraph(ConcurrentHashMap<String, Node> graph, String word) {
		 ArrayList<String> neighbors = findNeighbors(word); //got a list of word's neighbors
		 ArrayList<Node> nodes = new ArrayList<Node>();
		 Node startNode = new Node(word);
		 Node newNode;
		 for (String s : neighbors) { //change string list to node list
			 newNode = new Node(s);
			 nodes.add(newNode);
			 graph.putIfAbsent(s, newNode);
		 }
		 startNode.setList(nodes); //make sure node has neighbors
		 graph.putIfAbsent(word, startNode);
		 graph.get(word).setList(nodes);
		 return nodes;
	 }
	 public static void main(String[] args) {
			ThreeLetterWord tlw = new ThreeLetterWord();
	 }
}
