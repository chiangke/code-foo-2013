import java.io.File;
import java.io.FileNotFoundException;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

public class ThreeLetterWord {
	 Set<String> dictionary = new HashSet<String>();
	 Scanner scanner;
	 ConcurrentHashMap<String, HashSet> graph;
	 ConcurrentHashMap<String, Node> graph2;
	 static String alphabet = "abcdefghijklmnopqrstuvwxyz";
	 class Node {
		 String word;
		 Node predecessor;
		 ArrayList<Node> nodeList;
		 int depth;
		 boolean visited;
		 public Node(String s) {
			 word = s;
			 predecessor = null;
			 depth = -1;
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
		 public void setDepth(int i) {
			 depth = i;
		 }
		 public int getDepth() {
			 return depth;
		 }
		 public void setVisited() {
			 visited = true;
		 }
		 public boolean getVisited() {
			 return visited;
		 }
	 }
	 public ThreeLetterWord() {
		graph = new ConcurrentHashMap<String, HashSet>();
		graph2 = new ConcurrentHashMap<String, Node>();
		try {
			scanner = new Scanner(new File("three-letter-words.txt"));
		} catch(FileNotFoundException e) {
			System.out.println("Couldn't find file three-letter-words.txt");
		}
		while (scanner.hasNext()) {
			dictionary.add(scanner.nextLine().toLowerCase());
		}
		//for (String s : findNeighbors("cat")) {
		//	System.out.println(s);
		//}
		makeGraph(graph2, "dog");
		//ArrayList<String> temp = findNeighbors("dog");
		//for (String s : temp) {
		//	makeGraph(graph2, s);
		//}
		breadthfirstsearch(graph2, "dog", "cat");
		
	 }
	 public static void main(String[] args) {
		 ThreeLetterWord tlw = new ThreeLetterWord();
	 }
	 public void breadthfirstsearch(Map<String, Node> graph, String start, String end) {
		 ArrayDeque<Node> queue = new ArrayDeque<Node>();
		 ArrayList<Node> nodeList;
		 String temp;
		 boolean found = false;
		 //graph2.get(start).setVisited();
		 queue.add(graph2.get(start));
		 int numSwaps=0;
		 looking:
		 while(!queue.isEmpty()) {
			 temp = queue.pop().word;
			 System.out.println("TEMP IS " + temp);
			 if (!graph2.get(temp).getVisited()) {
				 //graph2.get(temp).setVisited();
				 //System.out.println("set visited");
				 if (temp.equals(end)) {
					 queue.clear();
					 break;
				 }
			 }
				 //if(!graph2.get(temp).getList().contains(end)) {
				 
						 nodeList = makeGraph(graph2, temp);
				 	numSwaps++;
				// }
				// else {
					 //nodeList = graph2.get(temp).getList();
				// }
				 for(Node n : nodeList) {
					 if (!graph2.get(n.word).getVisited()) {
						 System.out.println(n.word);
						 graph2.get(n.word).setPredecessor(graph2.get(temp));
						 graph2.get(n.word).setVisited();
						 queue.add(graph2.get(n.word));
						 if (n.word.equals(end)) {
							 queue.clear();
							 break looking;
						 }
					 }
				 }
				 System.out.println("----");
			 
		 }
		 if (graph2.get(end)==null) {
			 System.out.println("Could not find");
		 }
		 else {
			 System.out.print(start + " -> ");
			 String from = end;
			 Node endNode = graph2.get(from);
		     while (graph2.get(from).getPredecessor()!=null) {
		    	 if (!graph2.get(from).getPredecessor().word.equals(start)) {
		    		 //System.out.print(graph2.get(from).getPredecessor().word + " -> ");
		    		 queue.add(graph2.get(from).getPredecessor());
		    	 }
		    	 else
		    		 break;
		    	 from = graph2.get(from).getPredecessor().word;
		    }
		     while (!queue.isEmpty()) {
		         System.out.print(queue.pollLast().word + " -> ");
		     }
		     System.out.print(end);
		 }
	 }
	 public void mapNeighbors(String word) {
		ArrayList<Node> nodes = new ArrayList<Node>();
		Node temp;
		for (String s : findNeighbors(word)) {
			nodes.add(new Node(s));
		}
		temp = new Node(word);
		temp.setList(nodes);
		//graph.put(word, temp);
	 }
	 
	 public ArrayList<String> findNeighbors(String word) {
		 ArrayList<String> neighbors = new ArrayList<String>();
		 StringBuilder theWord = new StringBuilder(word);
		 StringBuilder newWord;
		 for (int i=0; i<3; i++) {
			 for (int j=0; j<26; j++) {
				 if (alphabet.charAt(j)!=theWord.charAt(i)) {
					 newWord = new StringBuilder(word);
					 newWord.setCharAt(i, alphabet.charAt(j));
					 if (dictionary.contains(newWord.toString())) {
						 //System.out.println(newWord.toString());
						 neighbors.add(newWord.toString());
					 }
				 }
			 }
		 }
		 return neighbors;
	 }
	
	 public void processNeighbors(ConcurrentHashMap<String, HashSet> graph, String word) {
		 ArrayList<String> neighbors = findNeighbors(word);
		 graph.putIfAbsent(word, new HashSet<String>());
		 graph.get(word).addAll(neighbors);
		 for (String n : neighbors) {
			 graph.putIfAbsent(n, new HashSet());
		 }
		 for (String n : neighbors) {
			 graph.get(n).add(word);
		 }
	 }
	 
	 public ArrayList<Node> makeGraph(ConcurrentHashMap<String, Node> graph2, String word) {
		 ArrayList<String> neighbors = findNeighbors(word);
		 ArrayList<Node> nodes = new ArrayList<Node>();
		 Node startNode = new Node(word);
		 Node newNode;
		 for (String s : neighbors) { //change string list to node list
			 newNode = new Node(s);
			 nodes.add(newNode);
			 graph2.putIfAbsent(s, newNode);
		 }
		 startNode.setList(nodes);
		 graph2.putIfAbsent(word, startNode);
		 graph2.get(word).setList(nodes);
		 return nodes;
	 }
}
