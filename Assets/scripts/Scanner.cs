using System;
using UnityEngine;

public class Scanner {
    private string text;
    private int index = 0;
    private int peekIndex = 0;

    public Scanner(string text) {
        this.text = text;
    }

    /// <summary>
    /// Checks if there's another number left without advancing the scanner.
    /// </summary>
    public bool hasNextNumber() {
        int i = skipNonDigits(index);
        return i < text.Length;;
    }

    public int nextInt() {
        index = skipNonDigits(index);
        peekIndex = index;
        while(peekIndex < text.Length && Char.IsDigit(text[peekIndex])) {
            peekIndex++;
        }
        int result = int.Parse(text.Substring(index, peekIndex - index));
        index = peekIndex;
        return result;
    }

    public float nextFloat() {
        index = skipNonDigits(index);
        peekIndex = index;
        while(peekIndex < text.Length && (Char.IsDigit(text[peekIndex]) || text[peekIndex] == '.')) {
            peekIndex++;
        }
        float result = float.Parse(text.Substring(index, peekIndex - index));
        index = peekIndex;
        return result;
    }

    /// <summary>
    /// Finds the end of the current line, then returns the entire next line.
    /// Note that this discards everything that hasn't been scanned in the current line.
    /// </summary>
    public string nextLine() {
        int i = findEndOfLine(index);
        int j = findEndOfLine(i + 1);
        if (j >= text.Length) {
            j--;
        }
        index = j;
        return text.Substring(i + 1, j - i);
    }

    private int findEndOfLine(int startingIndex) {
        while (startingIndex < text.Length && text[startingIndex] != '\n') {
            startingIndex++;
        }
        return startingIndex;
    }

    /// <summary>
    /// Finds the index of the next digit.
    /// </summary>
    private int skipNonDigits(int i) {
        while (i < text.Length && !Char.IsDigit(text[i])) {
            i++;
        }
        return i;
    }
}