const parser = new DOMParser();

const serializer = new XMLSerializer();

export function formatXml(xml: string) {
  let xmlDoc = parser.parseFromString(xml, 'application/xml');
  let rootElement = xmlDoc.documentElement;
  indentChildren(xmlDoc, rootElement, "\n", "\n  ");
  xml = serializer.serializeToString(xmlDoc);
  return xml;
}

function indentChildren(xmlDoc: Document, node: HTMLElement, prevPrefix: string, prefix: string) {
  let children = node.childNodes;
  let i;
  let prevChild = null;
  let prevChildType = 1;
  let child: any = null;
  let childType;
  for (i = 0; i < children.length; i++) {
    child = children[i];
    childType = child.nodeType;
    if (childType != 3) {
      if (prevChildType == 3) {
        // Update prev text node with correct indent
        prevChild.nodeValue = prefix;
      } else {
        // Create and insert text node with correct indent
        let textNode = xmlDoc.createTextNode(prefix);
        node.insertBefore(textNode, child);
        i++;
      }
      if (childType == 1) {
        let isLeaf = child.childNodes.length == 0 || child.childNodes.length == 1 && child.childNodes[0].nodeType != 1;
        if (!isLeaf) {
          indentChildren(xmlDoc, child, prefix, prefix + "  ");
        }
      }
    }
    prevChild = child;
    prevChildType =childType;
  }
  if (child != null) {
    // Previous level indentation after last child
    if (childType == 3) {
      child.nodeValue = prevPrefix;
    } else {
      let textNode = xmlDoc.createTextNode(prevPrefix);
      node.append(textNode);
    }
  }
}