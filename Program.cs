//*****************************************************************************
//** 2213. Longest Substring of One Repeating Character             leetcode **
//*****************************************************************************

/**
 * Note: The returned array must be malloced, assume caller calls free().
 */
typedef struct
{
    int left;
    int right;
    int prefix;
    int suffix;
    int best;
    char leftChar;
    char rightChar;
} Node;

static Node *tree;

static int maxInt(int a, int b)
{
    return a > b ? a : b;
}

static void mergeNode(int index)
{
    Node *cur = &tree[index];
    Node *left = &tree[index * 2];
    Node *right = &tree[index * 2 + 1];

    cur->left = left->left;
    cur->right = right->right;

    cur->leftChar = left->leftChar;
    cur->rightChar = right->rightChar;

    cur->prefix = left->prefix;
    cur->suffix = right->suffix;

    int leftLength = left->right - left->left + 1;
    int rightLength = right->right - right->left + 1;

    if (left->prefix == leftLength &&
        left->rightChar == right->leftChar)
    {
        cur->prefix = leftLength + right->prefix;
    }

    if (right->suffix == rightLength &&
        left->rightChar == right->leftChar)
    {
        cur->suffix = rightLength + left->suffix;
    }

    cur->best = maxInt(left->best, right->best);

    if (left->rightChar == right->leftChar)
    {
        cur->best = maxInt(cur->best,
                           left->suffix + right->prefix);
    }
}

static void build(char *s, int index, int left, int right)
{
    tree[index].left = left;
    tree[index].right = right;

    if (left == right)
    {
        tree[index].prefix = 1;
        tree[index].suffix = 1;
        tree[index].best = 1;
        tree[index].leftChar = s[left];
        tree[index].rightChar = s[left];

        return;
    }

    int mid = left + (right - left) / 2;

    build(s, index * 2, left, mid);
    build(s, index * 2 + 1, mid + 1, right);

    mergeNode(index);
}

static void update(int index, int position, char ch)
{
    if (tree[index].left == tree[index].right)
    {
        tree[index].leftChar = ch;
        tree[index].rightChar = ch;
        tree[index].prefix = 1;
        tree[index].suffix = 1;
        tree[index].best = 1;

        return;
    }

    int mid = tree[index].left +
              (tree[index].right - tree[index].left) / 2;

    if (position <= mid)
    {
        update(index * 2, position, ch);
    }
    else
    {
        update(index * 2 + 1, position, ch);
    }

    mergeNode(index);
}

int* longestRepeating(char* s,
                      char* queryCharacters,
                      int* queryIndices,
                      int queryIndicesSize,
                      int* returnSize)
{
    int n = (int)strlen(s);

    int *retVal = malloc(sizeof(int) * queryIndicesSize);

    tree = malloc(sizeof(Node) * n * 4);

    build(s, 1, 0, n - 1);

    for (int i = 0; i < queryIndicesSize; i++)
    {
        int position = queryIndices[i];
        char ch = queryCharacters[i];

        if (s[position] != ch)
        {
            s[position] = ch;
            update(1, position, ch);
        }

        retVal[i] = tree[1].best;
    }

    free(tree);

    *returnSize = queryIndicesSize;

    return retVal;
}