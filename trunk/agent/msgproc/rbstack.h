#pragma region MIT License
/*
 * Nohros Notify - Default message processor
 * Copyright (c) 2009 Nohros Systems Inc, http://www.nohros.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this 
 * software and associated documentation files (the "Software"), to deal in the Software 
 * without restriction, including without limitation the rights to use, copy, modify, merge, 
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
 * to whom the Software is furnished to do so, subject to the following conditions:
 * 	
 * The above copyright notice and this permission notice shall be included in all copies or 
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */
#pragma endregion

/* a structure that represents a node in the command stack. */
typedef struct RBNODE {
	char* data; /* the node data */
	char pi; /* the parameter identification */
	int size; /* the size of the node data */
	RBNODE* next; /* the next node in the chain */
}*PRBNODE;

/* a structure that represents the command stack */
typedef struct RBSTACK {
	PRBNODE head; /* pointer to the first node */
	PRBNODE sp; /* pointer to the last node */
	int length; /* the stack length */
}*PRBSTACK;